﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Transfermarkt.Models.Requests
{
    public class ClubScoredGoals
    {
        public int ClubId { get; set; }
        public int NumberOfScoredGoals { get; set; }
        public string ClubName { get; set; }
    }
}
