﻿using nea_prototype_full;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nea_ui_testing
{
    public partial class StudentManagementMenu : Form
    {
        private List<Class> allClasses;
        private List<User> usersFromSelection;
        public StudentManagementMenu()
        {
            InitializeComponent();
            DatabaseHelper dbh = new DatabaseHelper();
            allClasses = dbh.GetAllClasses();

            // fill dropdown and clear selection
            ClassPicker.DataSource = allClasses.Select(x => x.ClassName).ToArray();
            ClassPicker.SelectedIndex = -1;
        }

        private void GoBackToDashboard(object sender, EventArgs e)
        {
            Close();
        }

        private void GoToCreateStudentMenu(object sender, EventArgs e)
        {
            Hide();
            StudentCreator sc = new StudentCreator();

            // form closed events
            sc.Closed += (s, args) =>
            {
                Show();
            };
            sc.Show();
        }

        private void SearchForStudents_Click(object sender, EventArgs e)
        {    
            try
            {
                // as long as one field is filled
                if (NameField.TextLength != 0 || ClassPicker.SelectedIndex != -1)
                {
                    DatabaseHelper dbh = new DatabaseHelper();
                    // if only name field
                    if (ClassPicker.SelectedIndex == -1)
                    {
                        usersFromSelection = dbh.GetStudentsByFirstName(NameField.Text);
                    }
                    // if only class field
                    else if (NameField.TextLength == 0)
                    {
                        usersFromSelection = dbh.GetStudentsInClass(allClasses[ClassPicker.SelectedIndex]);
                    }
                    // if both fields
                    else
                    {
                        usersFromSelection = dbh.GetStudentsMultimetric(NameField.Text, allClasses[ClassPicker.SelectedIndex]);
                    }
                    StudentMatches.DataSource = usersFromSelection.Select(x => $"{x.Id}\t{x.FirstName} {x.Surname}").ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.Message);
                eh.DisplayErrorForm();
            }
        }

        private void UpdateStudentInformation(object sender, EventArgs e)
        {
            DatabaseHelper dbh = new DatabaseHelper();
            User selectedStudent = usersFromSelection[StudentMatches.SelectedIndex];
            NameLabel.Text = $"Name: {selectedStudent.FirstName} {selectedStudent.Surname}";
            EmailLabel.Text = $"Email: {selectedStudent.Email}";
            ClassLabel.Text = $"Classes: {string.Join(", ", dbh.GetClassesOfStudent(selectedStudent).Select(x => x.ClassName))}";
            LastLoginLabel.Text = $"Last Login: {dbh.GetLastLoginOfStudent(selectedStudent)}";
        }

        private void EditStudentEvent(object sender, EventArgs e)
        {
            Hide();
            StudentCreator sc = new StudentCreator(usersFromSelection[StudentMatches.SelectedIndex]);

            // form closed events
            sc.Closed += (s, args) =>
            {
                Show();
            };
            sc.Show();
        }
    }
}
