﻿using System;

namespace Transfermarkt.Models.Requests
{
    public class MatchSchedule
    {
        public int Id { get; set; }
        public string MatchGame { get; set; }
        public string GameStart { get; set; }
    }
}
