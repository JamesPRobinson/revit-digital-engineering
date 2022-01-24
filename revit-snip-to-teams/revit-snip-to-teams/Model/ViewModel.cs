using Utils.Logger;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace revit-snip-to-teams.Model
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Private Members
        private BitmapSource image_source;
        private ImageSource source;
        private bool can_execute;
        private ICommand capture_command;
        private ICommand close_command;
        private ICommand send_command;
        private Visibility scrollbar_visibility;
        private ScrollBarVisibility scrollbar_horizontal_visibility;
        private ScrollBarVisibility scrollbar_vertical_visibility;
        private string button_text;
        private string email_text;
        private string user_text;
        private string text_message;
        private string selected_sort;
        private Window window;
        #endregion

        public Logger logger;
        public UIApplication uiapp;
        public bool ImageCaptured = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, string> Sorts { get; set; }


        public BitmapSource ImageSource
        {
            get
            {
                return image_source;
            }
            set
            {
                image_source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
            }
        }

        public bool CanExecute
        {
            get
            {
                return can_execute;
            }
        }

        public Visibility ScrollbarVisibility
        {
            get
            {
                return scrollbar_visibility;
            }
            set
            {
                scrollbar_visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScrollbarVisibility"));
            }
        }

        public ScrollBarVisibility ScrollbarHorizontalVisibility
        {
            get
            {
                return scrollbar_horizontal_visibility;
            }
            set
            {
                scrollbar_horizontal_visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScrollbarHorizontalVisibility"));
            }
        }

        public ScrollBarVisibility ScrollbarVerticalVisibility
        {
            get
            {
                return scrollbar_vertical_visibility;
            }
            set
            {
                scrollbar_vertical_visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScrollbarVerticalVisibility"));
            }
        }

        public ICommand CaptureCommand
        {
            get
            {
                return capture_command ?? (capture_command = new CommandHandler(() => Capture(), () => CanExecute));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return close_command ?? (close_command = new CommandHandler(() => Close(), () => CanExecute));
            }
        }

        public ICommand SendCommand
        {
            get
            {
                return send_command ?? (send_command = new CommandHandler(() => SendMessage(), () => CanExecute));
            }
        }

        public ImageSource Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Source"));
            }
        }

        public string ButtonText
        {
            get
            {
                return button_text;
            }
            set
            {
                button_text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonText"));
            }
        }

        public string EmailText
        {
            get
            {
                return email_text;
            }
            set
            {
                email_text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EmailText"));
            }
        }

        public string UserText
        {
            get
            {
                return user_text;
            }
            set
            {
                user_text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserText"));
            }
        }

        public string MessageText
        {
            get
            {
                return text_message;
            }
            set
            {
                text_message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MessageText"));
            }
        }

        public string SelectedSort
        {
            get
            {
                return selected_sort;
            }
            set
            {
                selected_sort = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedSort"));
            }
        }

        public ViewModel(UIApplication _uiapp, Window _window)
        {
            uiapp = _uiapp;
            window = _window;
            logger = new Logger();
            can_execute = true;
            Sorts = new Dictionary<string, string>()
            {
                { "User Content Feedback", "https://aecom.webhook.office.com/webhookb2/eb8b18c9-9250-4e7e-aea9-bf9257836677@16ed5ab4-2b59-4e40-806d-8a30bdc9cf26/IncomingWebhook/f41fe9185ac34de2aa653596c539c148/99877555-1e3e-485c-aa0c-cb0564d77e7a" },
                { "User Script Bugs", "https://aecom.webhook.office.com/webhookb2/eb8b18c9-9250-4e7e-aea9-bf9257836677@16ed5ab4-2b59-4e40-806d-8a30bdc9cf26/IncomingWebhook/52bd0120d0734be8a57b17bd3ffbda20/99877555-1e3e-485c-aa0c-cb0564d77e7a" },
                { "User Script Feedback", "https://aecom.webhook.office.com/webhookb2/eb8b18c9-9250-4e7e-aea9-bf9257836677@16ed5ab4-2b59-4e40-806d-8a30bdc9cf26/IncomingWebhook/caaf4d3667c746c691d5d8d638391ed0/99877555-1e3e-485c-aa0c-cb0564d77e7a" }
            };
            UserText = EmailText = uiapp.Application.Username;
            ScrollbarVisibility = Visibility.Hidden;
            ScrollbarVerticalVisibility = ScrollbarHorizontalVisibility = ScrollBarVisibility.Hidden;
            ButtonText = "Capture";
        }


        public void Capture()
        {
            ImageCaptured = true;
            Source = Screenshot.Screenshot.CaptureRegion();
            ScrollbarVisibility = Visibility.Visible;
            ScrollbarHorizontalVisibility = ScrollbarVerticalVisibility = ScrollBarVisibility.Visible;
            ButtonText = "Recapture";
        }

        public void Close()
        {
            window.Close();
        }


        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(EmailText))
            {
                TaskDialog.Show("Email Missing", "Please enter email address to send message.");
                return;
            }
            string email = EmailText;
            if (!RegexUtils.IsValidEmail(email))
            {
                TaskDialog.Show("Invalid Email", "Please enter valid email address to send message.");
                return;
            }
            if (string.IsNullOrEmpty(SelectedSort))
            {
                TaskDialog.Show("Missing Field", "Sort field required.");
                return;
            }

            if (ImageCaptured)
            {
                string image_name = "Revit Digtital Tools Capture Image " + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png";
                string temp_image_path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), image_name);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Source));
                using (FileStream stream = new FileStream(temp_image_path, FileMode.Create))
                    encoder.Save(stream);
                if (File.Exists(temp_image_path))
                {
                    try
                    {
                        logger.PostCaptionTeams(email, UserText, MessageText, temp_image_path, SelectedSort);
                    }
                    catch (Exception ex)
                    {
                        logger.PostErrorTeams(UserText, ex.ToString(), string.Empty);
                    }
                }
                else
                {
                    var res = TaskDialog.Show("Image Parser Error", "Could not load image, send text only?", TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
                    if (res == TaskDialogResult.Yes)
                    {
                        logger.PostErrorTeams(UserText, MessageText, SelectedSort);
                    }
                }
            }
            else
            {
                logger.PostErrorTeams(UserText, MessageText, SelectedSort);
            }
            Close();
        }
    }
}
