﻿using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared.App;
using logicpos.shared.Classes.Others;
using System;

namespace logicpos
{
    public class EntryValidation : EntryTouch
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Reference parent EntryBox Label
        private Label _label;
        public Label Label
        {
            get { return _label; }
            set { _label = value; }
        }
        private Label _label2;
        public Label Label2
        {
            get { return _label2; }
            set { _label2 = value; }
        }
        private Label _label3;
        public Label Label3
        {
            get { return _label3; }
            set { _label3 = value; }
        }
        private string _initialLabelText;
        public string LabelText
        {
            get { return _initialLabelText; }
            set { _initialLabelText = value; }
        }
        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }
        private int _words;
        public int Words
        {
            get { return _words; }
            set { _words = value; }
        }
        private int _maxLength = 0;
        public int Maxlength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }
        private int _maxWords = 0;
        public int MaxWords
        {
            get { return (_maxWords); }
            set { _maxWords = value; }
        }
        //Propertir from Entry(Gtk)
        private bool _validated;
        public bool Validated
        {
            get { return _validated; }
            set
            {
                _validated = value;
                Utils.ValidateUpdateColors(this, _label, _validated, _label2, _label3);
            }
        }

        //Constructor
        public EntryValidation()
        {
            try
            {
                Validate();
                _required = false;
                _keyboardMode = KeyboardMode.None;

                //Events
                Changed += delegate { Validate(); };

                //Get Initial Label Text
                if (_label != null) _initialLabelText = _label.Text;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public EntryValidation(Window pSourceWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired)
            : this(pSourceWindow, pKeyboardMode, pRule, pRequired, 0, 0)
        {
        }

        public EntryValidation(Window pSourceWindow, KeyboardMode pKeyboardMode, string pRule, bool pRequired, int pMaxLength, int pMaxWords)
            : this()
        {
            _sourceWindow = pSourceWindow;
            _keyboardMode = pKeyboardMode;
            _rule = pRule;
            _required = pRequired;
            _maxLength = pMaxLength;
            _maxWords = pMaxWords;

            //Assign MaxLength to Be Entry MaxLength
            if (pMaxLength > 0) MaxLength = pMaxLength;
            //Events
            this.Changed += EntryValidation_Changed;
        }

        //Default FieldValidateValue is the Entry.Text
        public void Validate()
        {
            Validate(Text);
        }

        public void Validate(string pValue)
        {
            //Default FieldValidateValue is the Entry.Text
            _validated = SharedUtils.Validate(pValue, _rule, _required);
            Utils.ValidateUpdateColors(this, _label, _validated, _label2, _label3);
        }

        private void EntryValidation_Changed(object sender, EventArgs e)
        {
            if (_maxLength > 0 || _maxWords > 0)
            {
                ValidateMaxLenghtMaxWordsResult result = SharedUtils.ValidateMaxLenghtMaxWords(Text, _initialLabelText, _maxLength, _maxWords);
                Text = result.Text;
                Label.Text = result.LabelText;
                _length = result.Length;
                _words = result.Words;
            }
        }
    }
}