﻿using nea_prototype_full;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ListExtensionMethods;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace nea_ui_testing
{
    public partial class QuestionAttemptMenu : Form
    {
        // question fields
        private Question questionRef;
        private List<Question> questionList;
        private Random random = new Random();
        private Control[] MCA_CONTROLS;
        private Control[] FI_CONTROLS;
        private bool canSubmit = false;
        private Assignment assignmentRef = null;

        private DateTime timeQuestionOpened = DateTime.MinValue;

        private List<Image> questionImagesRef;

        private Image originalImage;
        private double zoomFactor = 1;

        private Form formReturn;

        // drawing fields
        private bool isDrawing = false;
        private Point mouseDownPosition;
        private Point mouseUpPosition;
        private Graphics drawing;
        private Pen pen = new Pen(Color.Black, 3);

        public QuestionAttemptMenu(List<Question> questionReference = null, Assignment assignmentRef = null, Form formReturnRef = null)
        {
            InitializeComponent();

            questionRef = questionReference.First();
            questionReference.RemoveAt(0);
            questionList = questionReference;
            this.assignmentRef = assignmentRef;

            QuestionsRemainingLabel.Text = questionReference.Count.ToString();

            timeQuestionOpened = DateTime.Now;

            MCA_CONTROLS = new Control[] { MCA_D, MCA_C, MCA_B, MCA_A, MCALabel };
            FI_CONTROLS = new Control[] { FI_4, FI_FIELD4, FI_3, FI_FIELD3, FI_2, FI_FIELD2, FI_1, FI_FIELD1, FILabel };

            isDrawing = false;

            formReturn = formReturnRef;

            LoadQuestionData();
        }

        private void LoadQuestionData()
        {
            canSubmit = false;
            SubmitButton.Enabled = canSubmit;

            try
            {
                DatabaseHelper dbh = new DatabaseHelper();
                List<Image> imageList = new List<Image>();

                if (!(questionRef is RandomlyGeneratedQuestion))
                {
                    AuthorLabel.Text = $"Author: {questionRef.Author.FirstName} {questionRef.Author.Surname}";
                    imageList = dbh.GetQuestionImages(questionRef);
                }
                else
                {
                    AuthorLabel.Text = "Randomly generated question";
                    if ((questionRef as RandomlyGeneratedQuestion).RgqImage != null) imageList.Add((questionRef as RandomlyGeneratedQuestion).RgqImage);
                }

                questionImagesRef = imageList;

                TopicLabel.Text = $"Topic: {questionRef.Topic.TopicName}";
                SubjectLabel.Text = $"Subject: {questionRef.Topic.Subject.SubjectName}";
                DifficultyLabel.Text = $"Difficulty: {questionRef.Difficulty}";

                QuestionContentBox.Text = questionRef.QuestionContent;

                if (questionRef.IsMc)
                {
                    List<string> allAnswers;
                    if (questionRef.McAnswers.Count > 3) allAnswers = questionRef.McAnswers.Take(3).ToList();
                    else allAnswers = questionRef.McAnswers.ToList();
                    // if a mc question, it can only have one answer
                    allAnswers.Add(questionRef.Answer.First());
                    string[] shuffledAnswers = allAnswers.RandomiseList().ToArray();

                    MCA_A.Text = shuffledAnswers[0];
                    MCA_B.Text = shuffledAnswers[1];
                    if (allAnswers.Count >= 3) MCA_C.Text = shuffledAnswers[2];
                    if (allAnswers.Count >= 4) MCA_D.Text = shuffledAnswers[3];

                    foreach (Control c in MCA_CONTROLS)
                    {
                        SetVisible(c);
                    }
                    if (questionRef.McAnswers.Count < 4)
                    {
                        // if less than 4 mca available, hide unavailable radio buttons
                        foreach (Control c in MCA_CONTROLS.Take(4 - allAnswers.Count)) SetHidden(c);
                    }

                    foreach (Control c in FI_CONTROLS)
                    {
                        SetHidden(c);
                    }
                }
                else
                {
                    List<string> allAnswers;
                    if (questionRef.Answer.Count > 4) allAnswers = questionRef.Answer.Take(4).ToList();
                    else allAnswers = questionRef.Answer.ToList();

                    string match = Regex.Match(allAnswers[0], @"(?<=<).+(?=>)").Value;
                    FI_FIELD1.Text = match != string.Empty ? match : "Answer 1";
                    if (allAnswers.Count >= 2)
                    {
                        match = Regex.Match(allAnswers[1], @"(?<=<).+(?=>)").Value;
                        FI_FIELD2.Text = match != string.Empty ? match : "Answer 2";
                    }
                    if (allAnswers.Count >= 3)
                    {
                        match = Regex.Match(allAnswers[2], @"(?<=<).+(?=>)").Value;
                        FI_FIELD3.Text = match != string.Empty ? match : "Answer 3";
                    }
                    if (allAnswers.Count == 4)
                    {
                        match = Regex.Match(allAnswers[3], @"(?<=<).+(?=>)").Value;
                        FI_FIELD4.Text = match != string.Empty ? match : "Answer 4";
                    }

                    foreach (Control c in FI_CONTROLS)
                    {
                        SetVisible(c);
                    }

                    if (allAnswers.Count < 4)
                    {
                        foreach (Control c in FI_CONTROLS.Take((4 - allAnswers.Count) * 2)) SetHidden(c);
                    }

                    foreach (Control c in MCA_CONTROLS)
                    {
                        SetHidden(c);
                    }
                }

                if (imageList.Count != 0)
                {
                    // sort out for more than one image
                    Image image = ResizeImageToWidth(imageList.First(), ImageBox.Width);
                    ImageBox.Height = image.Height;
                    ImageBox.Image = image;
                    ImageBox.SizeMode = PictureBoxSizeMode.CenterImage;

                    originalImage = image;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.Message);
                eh.DisplayErrorForm();
            }
        }

        private void GoBackToDashboard(object sender, EventArgs e)
        {
            Hide();
            ConfirmationForm cf = new ConfirmationForm($"Are you sure you want to quit? You will lose progress.");
            bool wasSuccess = false;

            // form closed events
            cf.FormClosing += (s, args) =>
            {
                wasSuccess = cf.wasSuccess;
            };
            cf.Closed += (s, args) =>
            {
                if (wasSuccess)
                {
                    Hide();
                    formReturn.Show();
                    Close();
                }
                else Show();
            };
            cf.Show();
        }

        private void SetVisible(Control c)
        {
            c.Visible = true;
            c.Enabled = true;
            c.BringToFront();
        }

        private void SetHidden(Control c)
        {
            c.Visible = false;
            c.Enabled = false;
            c.SendToBack();
        }

        private Image ResizeImageToWidth(Image image, int width)
        {
            double resizeRatio = image.Width / (double)width;
            return new Bitmap(image, new Size(width, (int)Math.Round(image.Height / resizeRatio)));
        }

        private void SubmitEvent(object sender, EventArgs e)
        {
            try
            {
                bool wasCorrect = false;
                string wholeStudentAnswer = string.Empty;
                if (questionRef.IsMc)
                {
                    string studentAnswer = this.Controls.OfType<RadioButton>().First(x => x.Checked).Text;
                    if (questionRef.Answer.First() == studentAnswer) wasCorrect = true;
                    wholeStudentAnswer = studentAnswer;
                }
                else
                {
                    List<string> studentAnswers = new List<string>();
                    foreach (TextBox tb in this.Controls.OfType<TextBox>().Where(x => x.Visible)) studentAnswers.Add(tb.Text.Replace(" ", ""));

                    List<string> questionAnsWithoutFields = new List<string>();
                    foreach (string answer in questionRef.Answer)
                    {
                        Match m = Regex.Match(answer, @".+(?=<.+>)");
                        if (m.Success) questionAnsWithoutFields.Add(m.Value.Replace(" ", ""));
                        else questionAnsWithoutFields.Add(answer.Replace(" ", ""));
                    }

                    wasCorrect = true;
                    foreach (string answer in studentAnswers)
                    {
                        if (!questionAnsWithoutFields.Contains(answer)) wasCorrect = false;
                    }
                    wholeStudentAnswer = string.Join(", ", studentAnswers);
                }

                DatabaseHelper dbh = new DatabaseHelper();
                if (questionRef is RandomlyGeneratedQuestion)
                {
                    dbh.InsertStudentQuestionAttemptWithTopic(questionRef.Topic, Program.loggedInUser, wasCorrect, wholeStudentAnswer, timeQuestionOpened, assignmentRef);
                }
                else
                {
                    dbh.InsertStudentQuestionAttempt(questionRef, Program.loggedInUser, wasCorrect, wholeStudentAnswer, timeQuestionOpened, assignmentRef);
                }

                SubmitButton.Enabled = false;
                DashboardButton.Enabled = false;

                Hide();
                InstantFeedbackForm iff = new InstantFeedbackForm(questionRef, questionList, wasCorrect, assignmentRef, formReturn);

                // form closed events
                iff.Load += (s, args) =>
                {
                    Close();
                };
                iff.Show();

                // add support for assignments
            }
            catch (Exception ex)
            {
                ErrorHandler eh = new ErrorHandler(ex.Message);
                eh.DisplayErrorForm();
            }
        }

        private void TestForData(object sender, EventArgs e)
        {
            if (questionRef.IsMc)
            {
                canSubmit = MCA_A.Checked || MCA_B.Checked || MCA_C.Checked || MCA_D.Checked;
            }
            else
            {
                canSubmit = true;
                foreach(TextBox tb in this.Controls.OfType<TextBox>().Where(x => x.Visible))
                {
                    if (tb.Text == string.Empty) canSubmit = false;
                }
            }
            SubmitButton.Enabled = canSubmit;
        }

        private void ZoomEvent(object sender, MouseEventArgs e)
        {
            if (originalImage != null)
            {
                if (e.Button.Equals(MouseButtons.Left) && zoomFactor < 4) zoomFactor *= 1.25;
                else if (e.Button.Equals(MouseButtons.Right) && zoomFactor > 0.4) zoomFactor *= 0.8;

                Size resize = new Size((int)(originalImage.Width * zoomFactor), (int)(originalImage.Height * zoomFactor));
                Image newImage = new Bitmap(originalImage, resize);

                ImageBox.Image = newImage;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            // create graphics object from drawing box background so that lines can be drawn on it
            DrawingBox.BackgroundImage = new Bitmap(DrawingBox.Width, DrawingBox.Height, PixelFormat.Format24bppRgb);
            drawing = Graphics.FromImage(DrawingBox.BackgroundImage);

            // smooth rendering
            drawing.SmoothingMode = SmoothingMode.AntiAlias;

            // set drawing box background to white
            drawing.Clear(Color.White);

            // smooth pen edges
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;

            // set first point for mouse move invocations
            mouseDownPosition = e.Location;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // draw a line from point of previous invocation to this invocation, refresh to apply changes
                mouseUpPosition = e.Location;
                drawing.DrawLine(pen, mouseDownPosition, mouseUpPosition);
                DrawingBox.Refresh();

                // set point of previous invocation to point of this invocation to set up next invocation
                mouseDownPosition = mouseUpPosition;
            }
        }

        private void ClearDrawing(object sender, EventArgs e)
        {
            // reset background to white and refresh to apply changes
            drawing.Clear(Color.White);
            DrawingBox.Refresh();
        }
    }
}
