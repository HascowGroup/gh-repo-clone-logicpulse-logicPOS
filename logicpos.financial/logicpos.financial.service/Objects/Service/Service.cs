﻿using logicpos.financial.service.App;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.financial.service.Objects.Service
{
    internal class Service : ServiceBase
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _debug = true;

        public Service()
        {
            if (_debug) _logger.Debug("Service Construct");
            ServiceName = Program.SERVICE_NAME;
        }

        public void Start(string[] args)
        {
            if (_debug) _logger.Debug("Service Start");
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            if (_debug) _logger.Debug("Service OnStart");
            Program.Start(args);
        }

        protected override void OnStop()
        {
            if (_debug) _logger.Debug("Service OnStop");
            Program.Stop();
        }
    }
}
