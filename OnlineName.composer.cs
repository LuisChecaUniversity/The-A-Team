// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TheATeam
{
    partial class OnlineName
    {
        ImageBox ImageBox_1;
        Label Label_1;
        EditableText EditableText_1;
        Button btnEnter;
        Button btnBack;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            EditableText_1 = new EditableText();
            EditableText_1.Name = "EditableText_1";
            btnEnter = new Button();
            btnEnter.Name = "btnEnter";
            btnBack = new Button();
            btnBack.Name = "btnBack";

            // OnlineName
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(EditableText_1);
            this.RootWidget.AddChildLast(btnEnter);
            this.RootWidget.AddChildLast(btnBack);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/BGPlain.png");
            ImageBox_1.ImageScaleType = ImageScaleType.Center;

            // Label_1
            Label_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // EditableText_1
            EditableText_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            EditableText_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            EditableText_1.LineBreak = LineBreak.Character;
            EditableText_1.HorizontalAlignment = HorizontalAlignment.Center;

            // btnEnter
            btnEnter.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnEnter.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // btnBack
            btnBack.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            btnBack.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    ImageBox_1.SetPosition(-144, -128);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Label_1.SetPosition(272, 143);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    EditableText_1.SetPosition(284, 188);
                    EditableText_1.SetSize(360, 56);
                    EditableText_1.Anchors = Anchors.None;
                    EditableText_1.Visible = true;

                    btnEnter.SetPosition(373, 324);
                    btnEnter.SetSize(214, 56);
                    btnEnter.Anchors = Anchors.None;
                    btnEnter.Visible = true;

                    btnBack.SetPosition(373, 448);
                    btnBack.SetSize(214, 56);
                    btnBack.Anchors = Anchors.None;
                    btnBack.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(960, 544);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Label_1.SetPosition(373, 62);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    EditableText_1.SetPosition(300, 188);
                    EditableText_1.SetSize(360, 56);
                    EditableText_1.Anchors = Anchors.None;
                    EditableText_1.Visible = true;

                    btnEnter.SetPosition(373, 323);
                    btnEnter.SetSize(214, 56);
                    btnEnter.Anchors = Anchors.None;
                    btnEnter.Visible = true;

                    btnBack.SetPosition(373, 449);
                    btnBack.SetSize(214, 56);
                    btnBack.Anchors = Anchors.None;
                    btnBack.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Enter Name";

            EditableText_1.Text = "Enter Name";
            EditableText_1.DefaultText = "Name";

            btnEnter.Text = "Enter";

            btnBack.Text = "Back";
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
