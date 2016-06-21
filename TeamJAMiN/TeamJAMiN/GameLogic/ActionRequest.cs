using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.GameLogic
{
    public class ActionRequest
    {
        public GameActionState State { get; set; }
        public string ActionLocation { get; set; }
    }
}