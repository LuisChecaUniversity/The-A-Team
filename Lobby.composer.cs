// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    partial class Lobby
    {
        ImageBox ImageBox_1;
        Panel pnlActivePlayers;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            pnlActivePlayers = new Panel();
            pnlActivePlayers.Name = "pnlActivePlayers";

            // Lobby
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(pnlActivePlayers);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGPlain.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // pnlActivePlayers
            pnlActivePlayers.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            pnlActivePlayers.Clip = true;

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

                    ImageBox_1.SetPosition(-88, -125);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    pnlActivePlayers.SetPosition(604, 140);
                    pnlActivePlayers.SetSize(100, 100);
                    pnlActivePlayers.Anchors = Anchors.None;
                    pnlActivePlayers.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    pnlActivePlayers.SetPosition(573, 46);
                    pnlActivePlayers.SetSize(360, 471);
                    pnlActivePlayers.Anchors = Anchors.None;
                    pnlActivePlayers.Visible = true;

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
