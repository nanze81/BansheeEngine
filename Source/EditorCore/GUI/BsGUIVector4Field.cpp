//********************************** Banshee Engine (www.banshee3d.com) **************************************************//
//**************** Copyright (c) 2016 Marko Pintera (marko.pintera@gmail.com). All rights reserved. **********************//
#include "GUI/BsGUIVector4Field.h"
#include "GUI/BsGUILayoutX.h"
#include "GUI/BsGUILayoutY.h"
#include "GUI/BsGUIFloatField.h"
#include "GUI/BsGUILabel.h"
#include "GUI/BsGUISpace.h"

using namespace std::placeholders;

namespace bs
{
	const UINT32 GUIVector4Field::ELEMENT_LABEL_WIDTH = 15;

	GUIVector4Field::GUIVector4Field(const PrivatelyConstruct& dummy, const GUIContent& labelContent, 
		UINT32 labelWidth, const String& style, const GUIDimensions& dimensions, bool withLabel)
		:TGUIField(dummy, labelContent, labelWidth, style, dimensions, withLabel), mFieldX(nullptr), mFieldY(nullptr)
	{
		mFieldX = GUIFloatField::create(HString("X"), ELEMENT_LABEL_WIDTH, getSubStyleName(getXFieldStyleType()));
		mFieldY = GUIFloatField::create(HString("Y"), ELEMENT_LABEL_WIDTH, getSubStyleName(getYFieldStyleType()));
		mFieldZ = GUIFloatField::create(HString("Z"), ELEMENT_LABEL_WIDTH, getSubStyleName(getZFieldStyleType()));
		mFieldW = GUIFloatField::create(HString("W"), ELEMENT_LABEL_WIDTH, getSubStyleName(getWFieldStyleType()));

		mFieldX->onValueChanged.connect(std::bind(&GUIVector4Field::valueChanged, this, _1));
		mFieldY->onValueChanged.connect(std::bind(&GUIVector4Field::valueChanged, this, _1));
		mFieldZ->onValueChanged.connect(std::bind(&GUIVector4Field::valueChanged, this, _1));
		mFieldW->onValueChanged.connect(std::bind(&GUIVector4Field::valueChanged, this, _1));

		mFieldX->onConfirm.connect(std::bind(&GUIVector4Field::inputConfirmed, this));
		mFieldY->onConfirm.connect(std::bind(&GUIVector4Field::inputConfirmed, this));
		mFieldZ->onConfirm.connect(std::bind(&GUIVector4Field::inputConfirmed, this));
		mFieldW->onConfirm.connect(std::bind(&GUIVector4Field::inputConfirmed, this));

		mLayout->removeElement(mLabel);

		GUILayout* layout = mLayout->addNewElement<GUILayoutY>();
		layout->addElement(mLabel);
		mLabel->resetDimensions();

		GUILayout* elementLayout = layout->addNewElement<GUILayoutX>();

		elementLayout->addElement(mFieldX);
		elementLayout->addNewElement<GUIFixedSpace>(5);
		elementLayout->addElement(mFieldY);
		elementLayout->addNewElement<GUIFixedSpace>(5);
		elementLayout->addElement(mFieldZ);
		elementLayout->addNewElement<GUIFixedSpace>(5);
		elementLayout->addElement(mFieldW);
	}

	Vector4 GUIVector4Field::getValue() const
	{
		Vector4 value;
		value.x = mFieldX->getValue();
		value.y = mFieldY->getValue();
		value.z = mFieldZ->getValue();
		value.w = mFieldW->getValue();

		return value;
	}

	void GUIVector4Field::setValue(const Vector4& value)
	{
		mFieldX->setValue(value.x);
		mFieldY->setValue(value.y);
		mFieldZ->setValue(value.z);
		mFieldW->setValue(value.w);
	}

	bool GUIVector4Field::hasInputFocus() const
	{
		return mFieldX->hasInputFocus() || mFieldY->hasInputFocus() || mFieldZ->hasInputFocus() || mFieldW->hasInputFocus();
	}

	void GUIVector4Field::setTint(const Color& color)
	{
		if (mLabel != nullptr)
			mLabel->setTint(color);

		mFieldX->setTint(color);
		mFieldY->setTint(color);
		mFieldZ->setTint(color);
		mFieldW->setTint(color);
	}

	void GUIVector4Field::styleUpdated()
	{
		if (mLabel != nullptr)
			mLabel->setStyle(getSubStyleName(getLabelStyleType()));

		mFieldX->setStyle(getSubStyleName(getXFieldStyleType()));
		mFieldY->setStyle(getSubStyleName(getYFieldStyleType()));
		mFieldZ->setStyle(getSubStyleName(getZFieldStyleType()));
		mFieldW->setStyle(getSubStyleName(getWFieldStyleType()));
	}

	void GUIVector4Field::valueChanged(float newValue)
	{
		Vector4 value = getValue();
		onValueChanged(value);
	}

	void GUIVector4Field::inputConfirmed()
	{
		onConfirm();
	}

	const String& GUIVector4Field::getGUITypeName()
	{
		static String typeName = "GUIVector4Field";
		return typeName;
	}

	const String& GUIVector4Field::getXFieldStyleType()
	{
		static String LABEL_STYLE_TYPE = "XFloatField";
		return LABEL_STYLE_TYPE;
	}

	const String& GUIVector4Field::getYFieldStyleType()
	{
		static String LABEL_STYLE_TYPE = "YFloatField";
		return LABEL_STYLE_TYPE;
	}

	const String& GUIVector4Field::getZFieldStyleType()
	{
		static String LABEL_STYLE_TYPE = "ZFloatField";
		return LABEL_STYLE_TYPE;
	}

	const String& GUIVector4Field::getWFieldStyleType()
	{
		static String LABEL_STYLE_TYPE = "WFloatField";
		return LABEL_STYLE_TYPE;
	}
}