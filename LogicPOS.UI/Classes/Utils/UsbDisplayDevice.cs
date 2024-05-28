﻿using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using logicpos.financial;
using logicpos.Resources.Localization;
using logicpos.financial.Classes.Orders;

namespace logicpos
{
    public class UsbDisplayDevice
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const bool _debug = false;
        private UsbDevice _usbDevice;
        private UsbEndpointWriter _usbWriter;
        private ErrorCode _usbErrorCode = ErrorCode.None;
        //private const int MAX_CHARS_PER_LINE = 20;
        private int _charactersPerLine = 20;
        private uint _standByInSeconds;
        private string _standByLine1 = string.Empty;
        private string _standByLine2 = string.Empty;
        //Timer
        private bool _timerRunning = false;
        private uint _writeAfterSecondsRemain = 0;
        private string _writeAfterLine1 = string.Empty;
        private string _writeAfterLine2 = string.Empty;
        private bool _writeAfterCentered = false;

        /// <summary>
        /// POS Display Device
        /// </summary>
        /// <param name="Vid">Vendor ID Hex:0x03EB, Int: 1003</param>
        /// <param name="Pid">Product ID Hex:0x1101, Int: 4353</param>
        /// <param name="WriteEndpointID">Usb EndPoint</param>
        public UsbDisplayDevice(int pVid, int pPid, string pWriteEndpoint)
            : this(pVid, pPid, GetEnumFromString(pWriteEndpoint))
        {
        }

        public UsbDisplayDevice(string pVid, string pPid, string pWriteEndpoint)
            : this(ConvertStringHexToInt(pVid), ConvertStringHexToInt(pPid), GetEnumFromString(pWriteEndpoint))
        {
        }

        public UsbDisplayDevice(int pVid, int pPid, WriteEndpointID pWriteEndpointID)
        {
            try
            {
                //Init Usb Finder
                UsbDeviceFinder usbFinder = new UsbDeviceFinder(pVid, pPid);

                // Find and open the usb device.
                _usbDevice = UsbDevice.OpenUsbDevice(usbFinder);

                // If the device is open and ready
                string message = string.Empty;
                if (_usbDevice == null)
                {
                    message = string.Format("UsbDisplayDevice: Device Not Found VID:{0} PID:{1}", pVid, pPid);
                    _log.Error(message);
                    //throw new Exception(message);
                }
                else
                {
                    message = string.Format("UsbDisplayDevice: Device Found VID:{0} PID:{1}", pVid, pPid);
                    _log.Debug(message);
                }

                // If this is a "whole" usb device (libusb-win32, linux libusb)
                // it will have an IUsbDevice interface. If not (WinUSB) the
                // variable will be null indicating this is an interface of a
                // device.
                IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used,
                    // the desired configuration and interface must be selected.

                    // Select config
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface
                    wholeUsbDevice.ClaimInterface(1);
                }

                // open write endpoint
                if (_usbDevice != null)
                {
                    _usbWriter = _usbDevice.OpenEndpointWriter(pWriteEndpointID);
                }

                //Init Display
                if (_usbWriter != null)
                {
                    InitializeDisplay();
                    if (_debug) SetCursorInOff(0x01);
                }
            }
            catch (Exception ex)
            {
                _log.Error((_usbErrorCode != ErrorCode.None ? _usbErrorCode + ":" : string.Empty) + ex.Message);
            }
        }

        public void Close()
        {
            if (_usbDevice != null)
            {
                if (_usbDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the
                    // 'wholeUsbDevice' variable will be null indicating this is
                    // an interface of a device; it does not require or support
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = _usbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface
                        wholeUsbDevice.ReleaseInterface(1);
                    }

                    _usbDevice.Close();
                }
                _usbDevice = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Writers

        public void Write(byte[] pOutput)
        {
            try
            {
                //Only Write if _usbWriter is Enabled else ignore all calls, this way we can call Write in Pos, and if Device is Missing it skips all writes
                if (_usbWriter != null)
                {
                    // write data, read data
                    int bytesWritten;
                    _usbErrorCode = _usbWriter.Write(pOutput, 2000, out bytesWritten);

                    //ErrorCode Enumeration
                    //http://libusbdotnet.sourceforge.net/V2/html/c3eab258-a324-25c8-68ac-06ecf6e0fe7f.htm
                    if (_usbErrorCode != ErrorCode.None)
                    {
                        Close();
                        // Write that output to the console.
                        _log.Error(UsbDevice.LastErrorString);
                        //throw new Exception(UsbDevice.LastErrorString);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void Write(string pOutput)
        {
            ClearDisplayScreen();

            if (pOutput.Length > _charactersPerLine * 2)
            {
                pOutput = pOutput.Substring(0, Convert.ToInt16(_charactersPerLine * 2));
            }

            byte[] output = GetBytes(pOutput);
            Write(output);
        }

        public void Write(string pOutput, int pLine)
        {
            if (pLine == 1)
            {
                MoveCursorToPosition(0x01, 0x01);
            }
            else
            {
                MoveCursorToPosition(0x01, 0x02);
            }

            if (pOutput.Length > _charactersPerLine)
            {
                pOutput = pOutput.Substring(0, Convert.ToInt16(_charactersPerLine));
            }

            ClearCursorLine();
            byte[] output = GetBytes(pOutput);
            Write(output);
        }

        public void WriteCentered(string pOutput, int pLine)
        {
            Write(TextCentered(pOutput, _charactersPerLine), pLine);
        }

        public void WriteJustified(string pLeft, string pRight, int pLine)
        {
            Write(TextJustified(pLeft, pRight, Convert.ToInt16(_charactersPerLine)), pLine);
        }

        public void WriteAfterSeconds(uint pStopAfterSeconds, string pLine1, string pLine2)
        {
            WriteAfterSeconds(pStopAfterSeconds, pLine1, pLine2, true);
        }

        public void WriteAfterSeconds(uint pStopAfterSeconds, string pLine1, string pLine2, bool pCentered)
        {
            //Prepare Members : Reset Clock
            _writeAfterSecondsRemain = pStopAfterSeconds * 1000;
            _writeAfterLine1 = pLine1;
            _writeAfterLine2 = pLine2;
            _writeAfterCentered = pCentered;

            //Start Clock Work if not started Yet
            if (! _timerRunning) StartClock();
        }

        public void EnableStandBy()
        {
            WriteAfterSeconds(_standByInSeconds, _standByLine1, _standByLine2, true);
        }

        public void WriteStandBy()
        {
            WriteCentered(_standByLine1, 1);
            WriteCentered(_standByLine2, 2);
        }
        
        private void StartClock()
        {
            _timerRunning = true;

            // Every second call `update_status' (1000 milliseconds)
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateClock));
        }

        private bool UpdateClock()
        {
            bool result = true;

            _writeAfterSecondsRemain = _writeAfterSecondsRemain - 1000;
            //_log.Debug(string.Format("_writeAfterSecondsRemain: [{0}]", _writeAfterSecondsRemain));

            if (_writeAfterSecondsRemain <= 0)
            {
                if (_writeAfterCentered)
                {
                    _writeAfterLine1 = TextCentered(_writeAfterLine1, _charactersPerLine);
                    _writeAfterLine2 = TextCentered(_writeAfterLine2, _charactersPerLine);
                }
                Write(_writeAfterLine1, 0);
                Write(_writeAfterLine2, 1);

                result =  false;
            }
            else
            {
                // returning true means that the timeout routine should be invoked
                // again after the timeout period expires. Returning false would
                // terminate the timeout.
                result =  true;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ESC/POS Commands

        public void MoveCursorRight()
        {
            Write(new byte[] { 0x09 });
        }

        public void MoveCursorLeft()
        {
            Write(new byte[] { 0x08 });
        }

        public void MoveCursorUp()
        {
            Write(new byte[] { 0x1F, 0x0A });
        }

        public void MoveCursorDown()
        {
            Write(new byte[] { 0x0A });
        }

        public void MoveCursorToRightMost()
        {
            Write(new byte[] { 0x1F, 0x0D });
        }

        public void MoveCursorToLeftMost()
        {
            Write(new byte[] { 0x0D });
        }

        public void MoveCursorToHome()
        {
            Write(new byte[] { 0x0B });
        }

        public void MoveCursorToBottom()
        {
            Write(new byte[] { 0x1F, 0x42 });
        }

        //x 01h ≦x≦14h, y=01h, 02h
        //0x01......0x09,0x0A...0x0F,0x10...,0x14
        public void MoveCursorToPosition(byte pX, byte pY)
        {
            Write(new byte[] { 0x1F, 0x24, pX, pY });
        }

        public void ClearCursorLine()
        {
            Write(new byte[] { 0x18 });
        }

        public void ClearDisplayScreen()
        {
            Write(new byte[] { 0x0C });
        }

        //01h ≦n≦04h (=brightest)
        public void BrightnessAdjustment(byte pValue)
        {
            Write(new byte[] { 0x1F, 0x58, pValue });
        }

        //00h ≦n≦ ffh
        public void BlinkDisplayScreen(byte pValue)
        {
            Write(new byte[] { 0x1F, 0x45, pValue });
        }

        public void InitializeDisplay()
        {
            Write(new byte[] { 0x1B, 0x40 });
        }

        //30h ≦n≦ 38h
        public void CommandTypeSelect(byte pValue)
        {
            Write(new byte[] { 0x1B, 0x23, pValue });
        }

        //00h ≦n≦ 0Ch
        public void SelectInternationalCharacterSet(byte pValue)
        {
            Write(new byte[] { 0x1B, 0x52, pValue });
        }

        //PT 0x10
        //n=00h, 01h..07h, 10h, 13h
        public void SelectCharacterCodeTable(byte pValue)
        {
            Write(new byte[] { 0x1B, 0x74, pValue });
        }

        //n=01 select, 00 cancel : n=00h, 01h
        public void SelectCancelReverseCharacter(byte pValue)
        {
            Write(new byte[] { 0x1F, 0x72, pValue });
        }

        //n=01 on, n=00 off : n=00h, 01h, 01h<m< 14h
        public void TurnAnnunciatorOnOff(byte pN, byte pM)
        {
            Write(new byte[] { 0x1F, 0x23, pN, pM });
        }

        //n=01 on, n=00 off : n=00h, 01h
        public void SetCursorInOff(byte pValue)
        {
            Write(new byte[] { 0x1F, 0x43, pValue });
        }

        public void SpecifyOverwriteMode()
        {
            Write(new byte[] { 0x1F, 0x01 });
        }

        public void SpecifyVerticalScrollMode()
        {
            Write(new byte[] { 0x1F, 0x02 });
        }

        public void SpecifyHorizontalScrollMode()
        {
            Write(new byte[] { 0x1F, 0x03 });
        }

        public void ExecuteSelfTest()
        {
            Write(new byte[] { 0x1F, 0x40 });
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Utils

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine)
        {
            return TextJustified(pLeft, pRight, pMaxPerLine, "{0,-10}{1,10}");
        }

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine, string pFormat)
        {
            return string.Format(pFormat, pLeft, pRight);
        }

        public static string TextCentered(string stringToCenter, int pCharactersPerLine)
        {
            return stringToCenter.PadLeft(((pCharactersPerLine - stringToCenter.Length) / 2) + stringToCenter.Length).PadRight(pCharactersPerLine);
        }

        public static string TextCentered(string stringToCenter, int totalLength, char paddingCharacter)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length, paddingCharacter).PadRight(totalLength, paddingCharacter);
        }

        public static WriteEndpointID GetEnumFromString(string pValue)
        {
            return (WriteEndpointID)Enum.Parse(typeof(WriteEndpointID), pValue, true);
        }

        public static int ConvertStringHexToInt(string pHex)
        {
            return Convert.ToInt32(pHex, 16);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>
        /// Returns a valid Display Object from Pos Settings or Null
        /// </summary>
        /// <returns></returns>
        public static object InitDisplay()
        {
            object result = null;

            bool hardwareDisplayEnabled = Convert.ToBoolean(GlobalFramework.Settings["hardwareDisplayEnabled"]);
            if (hardwareDisplayEnabled)
            {
                string hardwareDisplayVID = GlobalFramework.Settings["hardwareDisplayVID"];
                string hardwareDisplayPID = GlobalFramework.Settings["hardwareDisplayPID"];
                string hardwareDisplayEndPoint = GlobalFramework.Settings["hardwareDisplayEndPoint"];
                string hardwareDisplayCodeTable = GlobalFramework.Settings["hardwareDisplayCodeTable"];
                int hardwareDisplayCharactersPerLine = Convert.ToUInt16(GlobalFramework.Settings["hardwareDisplayCharactersPerLine"]);
                uint hardwareDisplayGoToStandByInSeconds = Convert.ToUInt16(GlobalFramework.Settings["hardwareDisplayGoToStandByInSeconds"]);
                string hardwareDisplayStandByLine1 = GlobalFramework.Settings["hardwareDisplayStandByLine1"];
                string hardwareDisplayStandByLine2 = GlobalFramework.Settings["hardwareDisplayStandByLine2"];

                //Init
                UsbDisplayDevice displayDevice = new UsbDisplayDevice(
                    hardwareDisplayVID,
                    hardwareDisplayPID,
                    hardwareDisplayEndPoint
                );
                //Initializers
                displayDevice._charactersPerLine = hardwareDisplayCharactersPerLine;
                displayDevice._standByInSeconds = hardwareDisplayGoToStandByInSeconds;
                displayDevice._standByLine1 = hardwareDisplayStandByLine1;
                displayDevice._standByLine2 = hardwareDisplayStandByLine2;

                result = displayDevice;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Pos Methods

        public void ShowOrder(OrderDetail pOrderDetail, int pIndex)
        {
            try
            {
                if (pOrderDetail.Lines.Count > 0)
                {
                    ShowOrder(
                        pOrderDetail.Lines[pIndex].Designation,
                        pOrderDetail.Lines[pIndex].Properties.Quantity,
                        pOrderDetail.Lines[pIndex].Properties.PriceFinal,
                        pOrderDetail.TotalFinal
                    );
                }
                else
                {
                    WriteStandBy();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void ShowOrder(string pArticle, decimal pQuantity, decimal pPrice, decimal pTotal)
        {
            string article = string.Format("{0} x {1}", FrameworkUtils.DecimalToString(pQuantity), pArticle);
            //string price = string.Format("{0}", FrameworkUtils.DecimalToString(pPrice));
            //string line1 = TextJustified(article, price, Convert.ToInt16(_charactersPerLine));
            Write(article, 1);
            WriteJustified(Resx.global_total, FrameworkUtils.DecimalToString(pTotal), 2);
            EnableStandBy();
        }

        public void ShowPayment(string pPaymentType, decimal pTotalDelivery, decimal pTotalChange)
        {
            Write(pPaymentType, 1);
            WriteJustified(FrameworkUtils.DecimalToString(pTotalDelivery), FrameworkUtils.DecimalToString(pTotalChange), 2);
            EnableStandBy();
        }
    }
}
