// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    partial class ECUIMainMenu
    {
        ImageBox ImageBox_1;
        Button btnSolo;
        Button btnOnline;
        Button btnDual;
        Button btnQuit;

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
            btnOnline = new Button();
            btnOnline.Name = "btnOnline";
            btnDual = new Button();
            btnDual.Name = "btnDual";
            btnQuit = new Button();
            btnQuit.Name = "btnQuit";

            // ECUIMainMenu
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(btnSolo);
            this.RootWidget.AddChildLast(btnOnline);
            this.RootWidget.AddChildLast(btnDual);
            this.RootWidget.AddChildLast(btnQuit);

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

            // btnOnline
            btnOnline.IconImage = null;
            btnOnline.Style = ButtonStyle.Custom;
            btnOnline.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/OnlineButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/OnlineButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };

            // btnDual
            btnDual.IconImage = null;
            btnDual.Style = ButtonStyle.Custom;
            btnDual.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/DualButton.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/DualButton.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(42, 27, 42, 27),
            };

            // btnQuit
            btnQuit.IconImage = null;
            btnQuit.Style = ButtonStyle.Custom;
            btnQuit.CustomImage = new CustomButtonImageSettings()
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

                    btnOnline.SetPosition(366, 242);
                    btnOnline.SetSize(214, 56);
                    btnOnline.Anchors = Anchors.None;
                    btnOnline.Visible = true;

                    btnDual.SetPosition(366, 242);
                    btnDual.SetSize(214, 56);
                    btnDual.Anchors = Anchors.None;
                    btnDual.Visible = true;

                    btnQuit.SetPosition(366, 242);
                    btnQuit.SetSize(214, 56);
                    btnQuit.Anchors = Anchors.None;
                    btnQuit.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    btnSolo.SetPosition(320, 200);
                    btnSolo.SetSize(320, 74);
                    btnSolo.Anchors = Anchors.None;
                    btnSolo.Visible = true;

                    btnOnline.SetPosition(320, 274);
                    btnOnline.SetSize(320, 74);
                    btnOnline.Anchors = Anchors.None;
                    btnOnline.Visible = true;

                    btnDual.SetPosition(320, 348);
                    btnDual.SetSize(320, 74);
                    btnDual.Anchors = Anchors.None;
                    btnDual.Visible = true;

                    btnQuit.SetPosition(320, 422);
                    btnQuit.SetSize(320, 84);
                    btnQuit.Anchors = Anchors.None;
                    btnQuit.Visible = true;

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
