using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryServerApp
{
    public class EventManager
    {
        public List<Event> EventList;
        private MainPage MainPage;

        //Die MainPage wird als Pointer mit übergeben dass wir aus den Events heraus in die Textbox schreiben können
        //dreckiger Hack, aber funktioniert und mir fällt nix besseres ein...
        public EventManager(MainPage link)
        {
            EventList = new List<Event>();
            MainPage = link;
        }

        public void CreateEvent(eventType type, string payload)
        {
            EventList.Add(new Event(type, payload, MainPage));
        }

        //Führt offene Events aus
        public void Run()
        {
            //Prüft ob Events da sind
            if (EventsPending() == true)
            {
                //Nimmt das älteste Event in der Liste und führt es solange immer wieder aus bis der 
                //Rückgabewert "true" ist = Event Abgeschlossen ist
                while (EventList[0].Run() == false) ;
                //Lösche das Event
                EventList.RemoveAt(0);

                //Hinweis: mit der letzten Referenz auf ein Objekt wird das Objekt automatisch gelöscht.
                //Die einzige Referenz (Pointer) der Events ist in der Liste. Lösche ich das Objekt in der
                //Liste mit RemoveAt(index) wird die Eventklasse tatsächlich gelöscht.
            }
        }

        private async Task PrintDebugMessageAsync(string input)
        {
            await MainPage.WriteToLogAsync(input);
            //sende die Nachricht an die Konsole und alle verbundenen Debug Clients
        }


        //Checkt ob es offene Events zum abarbeiten gibt
        public bool EventsPending()
        {
            if(EventList.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
