// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Preview
{
    partial class LobbyUI
    {
        ImageBox ImageBox_1;
        Panel Panel_1;
        Label Label_1;
        Panel Panel_2;
        Label lblLobbyChat;
        Button btnMainMenu;
        Button btnJoinGame;
        Label Label_2;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Panel_2 = new Panel();
            Panel_2.Name = "Panel_2";
            lblLobbyChat = new Label();
            lblLobbyChat.Name = "lblLobbyChat";
            btnMainMenu = new Button();
            btnMainMenu.Name = "btnMainMenu";
            btnJoinGame = new Button();
            btnJoinGame.Name = "btnJoinGame";
            Label_2 = new Label();
            Label_2.Name = "Label_2";

            // LobbyUI
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(Panel_1);
            this.RootWidget.AddChildLast(Panel_2);
            this.RootWidget.AddChildLast(Label_2);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGPlain.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 204f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(Label_1);

            // Label_1
            Label_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // Panel_2
            Panel_2.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 204f / 255f);
            Panel_2.Clip = true;
            Panel_2.AddChildLast(lblLobbyChat);
            Panel_2.AddChildLast(btnMainMenu);
            Panel_2.AddChildLast(btnJoinGame);

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

            // Label_2
            Label_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Center;

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

                    ImageBox_1.SetPosition(-155, -79);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Panel_1.SetPosition(578, 89);
                    Panel_1.SetSize(100, 100);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    Label_1.SetPosition(545, 73);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Panel_2.SetPosition(115, 105);
                    Panel_2.SetSize(100, 100);
                    Panel_2.Anchors = Anchors.None;
                    Panel_2.Visible = true;

                    lblLobbyChat.SetPosition(70, 180);
                    lblLobbyChat.SetSize(214, 36);
                    lblLobbyChat.Anchors = Anchors.None;
                    lblLobbyChat.Visible = true;

                    btnMainMenu.SetPosition(80, 427);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    btnJoinGame.SetPosition(280, 431);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

                    Label_2.SetPosition(333, 8);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Panel_1.SetPosition(564, 57);
                    Panel_1.SetSize(376, 466);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    Label_1.SetPosition(81, 19);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Panel_2.SetPosition(41, 57);
                    Panel_2.SetSize(462, 466);
                    Panel_2.Anchors = Anchors.None;
                    Panel_2.Visible = true;

                    lblLobbyChat.SetPosition(12, 19);
                    lblLobbyChat.SetSize(426, 300);
                    lblLobbyChat.Anchors = Anchors.None;
                    lblLobbyChat.Visible = true;

                    btnMainMenu.SetPosition(11, 381);
                    btnMainMenu.SetSize(214, 56);
                    btnMainMenu.Anchors = Anchors.None;
                    btnMainMenu.Visible = true;

                    btnJoinGame.SetPosition(237, 381);
                    btnJoinGame.SetSize(214, 56);
                    btnJoinGame.Anchors = Anchors.None;
                    btnJoinGame.Visible = true;

                    Label_2.SetPosition(373, 12);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Available Players";

            lblLobbyChat.Text = "Welcome";

            btnMainMenu.Text = "Main Menu";

            btnJoinGame.Text = "Join Game";

            Label_2.Text = "Lobby";
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
