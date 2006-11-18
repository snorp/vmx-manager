using System;
using Gtk;

namespace VmxManager {

    public class ConfigDialog : Dialog {

        private VirtualMachine machine;
        private DeviceView devview;
        private DeviceModel devmodel;

        [Glade.Widget]
        private Widget configDialogContent;

        [Glade.Widget]
        private Entry nameEntry;

        [Glade.Widget]
        private ComboBox guestOsCombo;

        [Glade.Widget]
        private SpinButton memorySpin;

        [Glade.Widget]
        private CheckButton soundToggle;

        [Glade.Widget]
        private Container deviceContent;

        public ConfigDialog (VirtualMachine machine, Window parent) :
            base ("Configure Virtual Machine", parent, DialogFlags.NoSeparator, Stock.Cancel, ResponseType.Cancel,
                  Stock.Ok, ResponseType.Ok) {

            this.machine = machine;

            Glade.XML xml = new Glade.XML ("vmx-manager.glade", "configDialogContent");
            xml.Autoconnect (this);

            guestOsCombo.Model = new OSModel ();
            
            CellRendererText renderer = new CellRendererText ();
            guestOsCombo.PackStart (renderer, false);
            guestOsCombo.AddAttribute (renderer, "text", 0);

            devview = new DeviceView ();
            devmodel = new DeviceModel (machine);
            devview.Model = devmodel;
            deviceContent.Add (devview);
            devview.Show ();

            VBox.Add (configDialogContent);
            DefaultWidth = 400;
            DefaultHeight = 300;

            Load ();
        }

        private void SetOsCombo () {
            TreeIter iter;

            ListStore model = (ListStore) guestOsCombo.Model;
            for (int i = 0; i < model.IterNChildren (); i++) {
                model.IterNthChild (out iter, i);

                GuestOperatingSystem os = (GuestOperatingSystem) model.GetValue (iter, 1);
                if (os.Equals (machine.OperatingSystem)) {
                    guestOsCombo.SetActiveIter (iter);
                }
            }
        }

        private void Load () {
            nameEntry.Text = machine.Name;
            memorySpin.Value = machine.MemorySize;
            soundToggle.Active = machine.SoundEnabled;

            SetOsCombo ();
        }

        private void Save () {
            machine.Name = nameEntry.Text;
            machine.MemorySize = (int) memorySpin.Value;
            machine.SoundEnabled = soundToggle.Active;

            TreeIter iter;
            if (guestOsCombo.GetActiveIter (out iter)) {
                GuestOperatingSystem os = (GuestOperatingSystem) guestOsCombo.Model.GetValue (iter, 1);
                machine.OperatingSystem = os;
            }
            
            machine.Save ();
        }

        protected override void OnResponse (ResponseType response) {
            if (response == ResponseType.Ok) {
                Save ();
            }

            Destroy ();
        }
    }
}