﻿using System;
using BansheeEngine;
 
namespace BansheeEditor
{
    /// <summary>
    /// Inspectable field displays GUI elements for a single <see cref="SerializableProperty"/>. This is a base class that
    /// should be specialized for all supported types contained by <see cref="SerializableProperty"/>. Inspectable fields
    /// can and should be created recursively - normally complex types like objects and arrays will contain fields of their 
    /// own, while primitive types like integer or boolean will consist of only a GUI element.
    /// </summary>
    public abstract class InspectableField
    {
        protected InspectableFieldLayout layout;
        protected SerializableProperty property;
        protected string title;
        protected int depth;
        protected SerializableProperty.FieldType type; 

        /// <summary>
        /// Property this field is displaying contents of.
        /// </summary>
        public SerializableProperty Property
        {
            get { return property; }
            set
            {
                if (value == null)
                    throw new ArgumentException("Cannot assign a null property to an inspectable field.");

                if (value.Type != type)
                {
                    throw new ArgumentException(
                        "Attempting to initialize an inspectable field with a property of invalid type.");
                }

                property = value;
            }
        }

        /// <summary>
        /// Creates a new inspectable field GUI for the specified property.
        /// </summary>
        /// <param name="title">Name of the property, or some other value to set as the title.</param>
        /// <param name="type">Type of property this field will be used for displaying.</param>
        /// <param name="depth">Determines how deep within the inspector nesting hierarchy is this field. Some fields may
        ///                     contain other fields, in which case you should increase this value by one.</param>
        /// <param name="layout">Parent layout that all the field elements will be added to.</param>
        /// <param name="property">Serializable property referencing the array whose contents to display.</param>
        public InspectableField(string title, SerializableProperty.FieldType type, 
            int depth, InspectableFieldLayout layout, SerializableProperty property)
        {
            this.layout = layout;
            this.title = title;
            this.type = type;
            this.depth = depth;

            Property = property;
        }

        /// <summary>
        /// Checks if contents of the field have been modified, and updates them if needed.
        /// </summary>
        /// <param name="layoutIndex">Index in the parent's layout at which to insert the GUI elements for this field.
        ///                           </param>
        /// <returns>State representing was anything modified between two last calls to <see cref="Refresh"/>.</returns>
        public virtual InspectableState Refresh(int layoutIndex)
        {
            return InspectableState.NotModified;
        }

        /// <summary>
        /// Returns the total number of GUI elements in the field's layout.
        /// </summary>
        /// <returns>Number of GUI elements in the field's layout.</returns>
        public int GetNumLayoutElements()
        {
            return layout.NumElements;
        }

        /// <summary>
        /// Returns an optional title layout. Certain fields may contain separate title and content layouts. Parent fields
        /// may use the separate title layout instead of the content layout to append elements. Having a separate title 
        /// layout is purely cosmetical.
        /// </summary>
        /// <returns>Title layout if the field has one, null otherwise.</returns>
        public virtual GUILayoutX GetTitleLayout()
        {
            return null;
        }

        /// <summary>
        /// Initializes the GUI elements for the field.
        /// </summary>
        /// <param name="layoutIndex">Index at which to insert the GUI elements.</param>
        protected internal abstract void Initialize(int layoutIndex);

        /// <summary>
        /// Destroys all GUI elements in the inspectable field.
        /// </summary>
        public virtual void Destroy()
        {
            layout.DestroyElements();
        }

        /// <summary>
        /// Creates a new inspectable field, automatically detecting the most appropriate implementation for the type
        /// contained in the provided serializable property. This may be one of the built-in inspectable field implemetations
        /// (like ones for primitives like int or bool), or a user defined implementation defined with a 
        /// <see cref="CustomInspector"/> attribute.
        /// </summary>
        /// <param name="title">Name of the property, or some other value to set as the title.</param>
        /// <param name="layoutIndex">Index into the parent layout at which to insert the GUI elements for the field .</param>
        /// <param name="depth">Determines how deep within the inspector nesting hierarchy is this field. Some fields may
        ///                     contain other fields, in which case you should increase this value by one.</param>
        /// <param name="layout">Parent layout that all the field elements will be added to.</param>
        /// <param name="property">Serializable property referencing the array whose contents to display.</param>
        /// <returns>Inspectable field implementation that can be used for displaying the GUI for a serializable property
        ///          of the provided type.</returns>
        public static InspectableField CreateInspectable(string title, int layoutIndex, int depth, 
            InspectableFieldLayout layout, SerializableProperty property)
        {
            InspectableField field = null;

            Type customInspectable = InspectorUtility.GetCustomInspectable(property.InternalType);
            if (customInspectable != null)
            {
                field = (InspectableField) Activator.CreateInstance(customInspectable, depth, title, property);
            }
            else
            {
                switch (property.Type)
                {
                    case SerializableProperty.FieldType.Int:
                        field = new InspectableInt(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Float:
                        field = new InspectableFloat(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Bool:
                        field = new InspectableBool(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Color:
                        field = new InspectableColor(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.String:
                        field = new InspectableString(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Vector2:
                        field = new InspectableVector2(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Vector3:
                        field = new InspectableVector3(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Vector4:
                        field = new InspectableVector4(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.ResourceRef:
                        field = new InspectableResourceRef(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.GameObjectRef:
                        field = new InspectableGameObjectRef(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Object:
                        field = new InspectableObject(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Array:
                        field = new InspectableArray(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.List:
                        field = new InspectableList(title, depth, layout, property);
                        break;
                    case SerializableProperty.FieldType.Dictionary:
                        field = new InspectableDictionary(title, depth, layout, property);
                        break;
                }
            }

            if (field == null)
                throw new Exception("No inspector exists for the provided field type.");

            field.Initialize(layoutIndex);
            field.Refresh(layoutIndex);

            return field;
        }
    }
}