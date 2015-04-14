// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    partial class LobbyUI
    {
        ImageBox ImageBox_1;
        Panel pnlActivePlayers;

		

        Panel pnlLobbyChat;
        Label lblLobbyChat;

		

        Button btnMainMenu;
        Button btnJoinGame;

		
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
            pnlLobbyChat = new Panel();
            pnlLobbyChat.Name = "pnlLobbyChat";
            lblLobbyChat = new Label();
            lblLobbyChat.Name = "lblLobbyChat";
            btnMainMenu = new Button();
            btnMainMenu.Name = "btnMainMenu";
            btnJoinGame = new Button();
            btnJoinGame.Name = "btnJoinGame";

            // LobbyUI
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(pnlActivePlayers);
            this.RootWidget.AddChildLast(pnlLobbyChat);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGPlain.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // pnlActivePlayers
            pnlActivePlayers.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            pnlActivePlayers.Clip = true;

            // pnlLobbyChat
            pnlLobbyChat.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            pnlLobbyChat.Clip = true;
            pnlLobbyChat.AddChildLast(lblLobbyChat);
            pnlLobbyChat.AddChildLast(btnMainMenu);
            pnlLobbyChat.AddChildLast(btnJoinGame);

            // lblLobbyChat
            lblLobbyChat.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            lblLobbyChat.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            lblLobbyChat.LineBreak = LineBreak.Character;
            lblLobbyChat.VerticalAlignment = VerticalAlignment.Top;

            // btnMainMenu
            btnMainMenu.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnMainMenu.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // btnJoinGame
            btnJoinGame.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnJoinGame.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    pnlLobbyChat.SetPosition(100, 113);
                    pnlLobbyChat.SetSize(100, 100);
                    pnlLobbyChat.Anchors = Anchors.None;
                    pnlLobbyChat.Visible = true;

                    lblLobbyChat.SetPosition(50, 159);
                    lblLobbyChat.SetSize(214, 36);
                    lblLobbyChat.Anchors = Anchors.None;
                    lblLobbyChat.Visible = true;

                    btnMainMenu.SetPosition(0, 373);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    btnJoinGame.SetPosition(230, 402);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

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

                    pnlLobbyChat.SetPosition(50, 66);
                    pnlLobbyChat.SetSize(453, 436);
                    pnlLobbyChat.Anchors = Anchors.None;
                    pnlLobbyChat.Visible = true;

                    lblLobbyChat.SetPosition(18, 28);
                    lblLobbyChat.SetSize(403, 248);
                    lblLobbyChat.Anchors = Anchors.None;
                    lblLobbyChat.Visible = true;

                    btnMainMenu.SetPosition(7, 336);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    btnJoinGame.SetPosition(227, 336);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            lblLobbyChat.Text = "Welcome";

            btnMainMenu.Text = "Main Menu";

            btnJoinGame.Text = "Join Game";
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
