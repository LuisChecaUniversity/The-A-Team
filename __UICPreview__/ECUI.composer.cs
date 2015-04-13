// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Preview
{
    partial class ECUI
    {
        ImageBox ImageBox_1;
        Button btnSolo;
        Button Button_1;
        Button Button_2;
        Button Button_3;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            btnSolo = new Button();
            btnSolo.Name = "btnSolo";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";
            Button_3 = new Button();
            Button_3.Name = "Button_3";

            // ECUI
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(btnSolo);
            this.RootWidget.AddChildLast(Button_1);
            this.RootWidget.AddChildLast(Button_2);
            this.RootWidget.AddChildLast(Button_3);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGMainMenu.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // btnSolo
            btnSolo.IconImage = null;
            btnSolo.Style = ButtonStyle.Custom;
            btnSolo.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/SoloButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/SoloButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };
            btnSolo.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // Button_1
            Button_1.IconImage = null;
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/OnlineButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/OnlineButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };

            // Button_2
            Button_2.IconImage = null;
            Button_2.Style = ButtonStyle.Custom;
            Button_2.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/DualButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/DualButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };

            // Button_3
            Button_3.IconImage = null;
            Button_3.Style = ButtonStyle.Custom;
            Button_3.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/QuitButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/QuitButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 544;
                    this.DesignHeight = 960;

                    ImageBox_1.SetPosition(-108, -100);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    btnSolo.SetPosition(366, 242);
                    btnSolo.SetSize(214, 56);
                    btnSolo.Anchors = Anchors.None;
                    btnSolo.Visible = true;

                    Button_1.SetPosition(366, 242);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(366, 242);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    Button_3.SetPosition(366, 242);
                    Button_3.SetSize(214, 56);
                    Button_3.Anchors = Anchors.None;
                    Button_3.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    btnSolo.SetPosition(345, 200);
                    btnSolo.SetSize(269, 74);
                    btnSolo.Anchors = Anchors.None;
                    btnSolo.Visible = true;

                    Button_1.SetPosition(345, 274);
                    Button_1.SetSize(269, 74);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(345, 348);
                    Button_2.SetSize(269, 74);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    Button_3.SetPosition(345, 422);
                    Button_3.SetSize(269, 74);
                    Button_3.Anchors = Anchors.None;
                    Button_3.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

    }
}
