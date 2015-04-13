// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    partial class OnlineHostJoin
    {
        ImageBox ImageBox_1;
        Button btnHostGame;
        Button btnJoinGame;
        Button btnMainMenu;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            btnHostGame = new Button();
            btnHostGame.Name = "btnHostGame";
            btnJoinGame = new Button();
            btnJoinGame.Name = "btnJoinGame";
            btnMainMenu = new Button();
            btnMainMenu.Name = "btnMainMenu";

            // OnlineHostJoin
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(btnHostGame);
            this.RootWidget.AddChildLast(btnJoinGame);
            this.RootWidget.AddChildLast(btnMainMenu);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGPlain.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // btnHostGame
            btnHostGame.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnHostGame.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // btnJoinGame
            btnJoinGame.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnJoinGame.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // btnMainMenu
            btnMainMenu.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnMainMenu.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    ImageBox_1.SetPosition(-122, -97);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    btnHostGame.SetPosition(85, 250);
                    btnHostGame.SetSize(214, 56);
                    btnHostGame.Anchors = Anchors.None;
                    btnHostGame.Visible = true;

                    btnJoinGame.SetPosition(614, 244);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

                    btnMainMenu.SetPosition(363, 442);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    btnHostGame.SetPosition(160, 244);
                    btnHostGame.SetSize(214, 56);
                    btnHostGame.Anchors = Anchors.None;
                    btnHostGame.Visible = true;

                    btnJoinGame.SetPosition(588, 244);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

                    btnMainMenu.SetPosition(373, 441);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            btnHostGame.Text = "Host Game";

            btnJoinGame.Text = "JoinGame";

            btnMainMenu.Text = "Main Menu";
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
