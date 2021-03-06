﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Transfermarkt.Models;
using Transfermarkt.Models.Requests;
using Transfermarkt.WinUI.Helper;

namespace Transfermarkt.WinUI.Forms
{
    public partial class FrmMatch : Form
    {
        private readonly APIService _aPIServiceStadium = new APIService("Stadiums");
        private readonly APIService _aPIServiceLeagues = new APIService("Leagues");
        private readonly APIService _aPIServiceClub = new APIService("Clubs");
        private readonly APIService _aPIServiceMatch = new APIService("Matches");
        private readonly APIService _aPIServiceReferee = new APIService("Referee");

        private int StadiumId { get; set; }
        private int SeasonId { get; set; }

        public FrmMatch()
        {
            InitializeComponent();
            TimePicker.Format = DateTimePickerFormat.Time;
            TimePicker.ShowUpDown = true;
            this.AutoValidate = AutoValidate.Disable;
        }
        private async void FrmMatch_Load(object sender, EventArgs e)
        {
            var leagues = await _aPIServiceLeagues.Get<List<Leagues>>();
            if (leagues.Count == 0)
            {
                MessageBox.Show("We don't have leagues", "Error");
                return;
            }
            leagues.Insert(0, new Leagues());
            CmbLeagues.DataSource = leagues;
            CmbLeagues.DisplayMember = "Name";
            CmbLeagues.ValueMember = "Id";

            CmbAwayClub.Enabled = false;
            CmbHomeClub.Enabled = false;
            CmbReferees.Enabled = false;
        }
        private async void CmbLeagues_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var leagueId = int.Parse(CmbLeagues.SelectedValue.ToString());

            CmbAwayClub.Enabled = true;
            CmbHomeClub.Enabled = true;
            CmbReferees.Enabled = true;
            DatePicker.Enabled = true;
            TimePicker.Enabled = true;
            pictureBox1.Image = null;
            pictureBox2.Image = null;

            var clubs = await _aPIServiceClub.GetById<List<ClubsLeague>>(leagueId, "ClubsInLeague");

            if (clubs.Count == 0)
            {
                MessageBox.Show("There are no clubs in league", "Information");
                return;
            }

            List<Clubs> comboHomeTeam = new List<Clubs>();
            List<Clubs> comboAwayTeam = new List<Clubs>();

            foreach (var item in clubs)
            {
                var clubInDb = await _aPIServiceClub.GetById<Clubs>(item.ClubId);
                comboHomeTeam.Add(clubInDb);
                comboAwayTeam.Add(clubInDb);
                SeasonId = item.SeasonId;
            }

            comboHomeTeam.Insert(0, new Clubs());
            CmbHomeClub.DataSource = comboHomeTeam;
            CmbHomeClub.DisplayMember = "Name";
            CmbHomeClub.ValueMember = "Id";

            comboAwayTeam.Insert(0, new Clubs());
            CmbAwayClub.DataSource = comboAwayTeam;
            CmbAwayClub.DisplayMember = "Name";
            CmbAwayClub.ValueMember = "Id";

            var referees = await _aPIServiceReferee.Get<List<Referees>>();
            if (referees.Count == 0)
            {
                MessageBox.Show("We don't have refeeres.", "Error");
                return;
            }
            List<PersonDropDownList> refereesDropDown = new List<PersonDropDownList>();
            foreach (var item in referees)
            {
                refereesDropDown.Add(new PersonDropDownList
                {
                    FullName = $"{item.FirstName} {item.LastName}",
                    Id = item.Id
                });
            }
            refereesDropDown.Insert(0, new PersonDropDownList());
            CmbReferees.DataSource = refereesDropDown;
            CmbReferees.DisplayMember = "FullName";
            CmbReferees.ValueMember = "Id";

            TxtStadium.Text = "Home stadium will load automatically.";
        }
        private async void CmbHomeClub_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var clubId = int.Parse(CmbHomeClub.SelectedValue.ToString());
            var homeClub = await _aPIServiceClub.GetById<Clubs>(clubId);

            if (homeClub != null)
            {
                Image image = ImageResizer.ByteArrayToImage(homeClub.Logo);
                var newImage = ImageResizer.ResizeImage(image, 200, 200);
                pictureBox1.Image = newImage;
                var homeStadium = await _aPIServiceStadium.GetById<Stadiums>(homeClub.Id, "HomeStadium");
                if (homeStadium == null)
                {
                    TxtStadium.Text = "Home club doesn't have home stadium.";
                    return;
                }
                StadiumId = homeStadium.Id;
                TxtStadium.Text = homeStadium.Name;
            }
        }
        private async void CmbAwayClub_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var clubId = int.Parse(CmbAwayClub.SelectedValue.ToString());
            var awayClub = await _aPIServiceClub.GetById<Clubs>(clubId);

            if (awayClub != null)
            {
                Image image = ImageResizer.ByteArrayToImage(awayClub.Logo);
                var newImage = ImageResizer.ResizeImage(image, 200, 200);
                pictureBox2.Image = newImage;
            }
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                try
                {
                    if (DatePicker.Value > DateTime.Now.AddDays(2))
                    {
                        if (CmbHomeClub.SelectedValue.ToString() == CmbAwayClub.SelectedValue.ToString())
                        {
                            MessageBox.Show("You can't pick two same teams to play against each other.", "Information", MessageBoxButtons.OK);
                            return;
                        }
                        var match = new Matches
                        {
                            HomeClubId = int.Parse(CmbHomeClub.SelectedValue.ToString()),
                            AwayClubId = int.Parse(CmbAwayClub.SelectedValue.ToString()),
                            DateGame = DatePicker.Value,
                            IsFinished = false,
                            StadiumId = StadiumId,
                            GameStart = TimePicker.Value.ToString(),
                            GameEnd = TimePicker.Value.AddHours(2).ToString(),
                            LeagueId = int.Parse(CmbLeagues.SelectedValue.ToString()),
                            SeasonId = SeasonId
                        };

                        var addedMatch = await _aPIServiceMatch.Insert<Matches>(match);

                        if (addedMatch != null)
                        {
                            var refereeMatch = new RefereeMatches
                            {
                                RefereeId = int.Parse(CmbReferees.SelectedValue.ToString()),
                                MatchId = addedMatch.Id
                            };
                            await _aPIServiceMatch.Insert<RefereeMatches>(refereeMatch, "RefereeMatch");

                            MessageBox.Show("Match added.", "Information", MessageBoxButtons.OK);
                            Close();
                            return;
                        }
                    }
                    MessageBox.Show("Match needs to be scheduled at least 3 days earlier.", "Information", MessageBoxButtons.OK);
                    return;
                }
                catch (Exception)
                {
                    MessageBox.Show("Error", "Something went wrong", MessageBoxButtons.OK);
                    throw;
                }
            }
        }
        private void CmbReferees_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CmbReferees.SelectedIndex == 0 || CmbReferees.SelectedIndex == -1)
            {
                errorProvider.SetError(CmbReferees, "You need to select option from combo box");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(CmbReferees, null);
            }
        }
        private void CmbHomeClub_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CmbHomeClub.SelectedIndex == 0 || CmbHomeClub.SelectedIndex == -1)
            {
                errorProvider.SetError(CmbHomeClub, "You need to select option from combo box");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(CmbHomeClub, null);
            }
        }
        private void CmbAwayClub_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CmbAwayClub.SelectedIndex == 0 || CmbAwayClub.SelectedIndex == -1)
            {
                errorProvider.SetError(CmbAwayClub, "You need to select option from combo box");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(CmbAwayClub, null);
            }
        }
    }
}
