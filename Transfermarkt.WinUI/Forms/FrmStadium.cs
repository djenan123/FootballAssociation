﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Transfermarkt.Models;

namespace Transfermarkt.WinUI.Forms
{
    public partial class FrmStadium : Form
    {
        private readonly APIService _aPIServiceStadium = new APIService("Stadiums");
        public string ClubName { get; set; }
        public int Id { get; set; }

        public FrmStadium(int id, string clubName)
        {
            InitializeComponent();
            Id = id;
            ClubName = clubName;
        }

        private async void FrmStadium_Load(object sender, EventArgs e)
        {
            lblClubName.Text = ClubName;

            try
            {
                var _stadium = await _aPIServiceStadium.GetById<Stadiums>(Id, "HomeStadium");
                if (_stadium != null)
                {
                    txtCapacity.Text = _stadium.Capacity.ToString();
                    txtDateBuilt.Text = _stadium.DateBuilt.ToString();
                    txtStadiumName.Text = _stadium.Name;
                    txtStadiumId.Text = _stadium.Id.ToString();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        readonly Stadiums stadium = new Stadiums();
        private async void BtnSaveStadium_Click(object sender, EventArgs e)
        {
            stadium.Capacity = int.Parse(txtCapacity.Text);
            stadium.ClubId = Id;
            stadium.DateBuilt = DateTime.Parse(txtDateBuilt.Text.ToString());
            stadium.Name = txtStadiumName.Text;
            if (string.IsNullOrWhiteSpace(txtStadiumId.Text))
                stadium.Id = 0;
            else
                stadium.Id = int.Parse(txtStadiumId.Text.ToString());

            if (stadium.Id == 0)
            {
                await _aPIServiceStadium.Insert<Stadiums>(stadium);
                MessageBox.Show("Successfully added!", "Stadium added");
                Close();
            }
            else
            {
                await _aPIServiceStadium.Update<Stadiums>(stadium, stadium.Id.ToString());
                MessageBox.Show("Successfully updated!", "Stadium updated");
                Close();
            }
        }
    }
}
