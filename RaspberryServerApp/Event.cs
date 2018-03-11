using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;


namespace RaspberryServerApp
{
    public enum eventType
    {
        IncomingTCPMessage,
        IncomingUserMessage,
        Command,
        AdminCommand,
        Broadcast,
        GroupMessage,
        PrivateMessage,
        Error,
        DebugMessage
    }

    public class Event
    {
        //Ein Event kann alles sein. Eine Nachricht, ein Kommando, eine Fehlermeldung oder Exception.
        //Alles wird ein Event erstellen welches dann hier abgearbeitet wird.
        //Meistens sind Events empfangene Nachrichten von anderen Nutzern, hier wird herausgefunden was
        //der Nutzer von uns will und wie wir darauf reagieren. Dann wirds gemacht und das Event als
        //EventFinished = true automatisch vom EventManager gelöscht.

        private User User;
        private bool EventFinished;
        private eventType EventType;
        private string EventContent;
        private string Payload;
        private MainPage MainPage;

        public Event(eventType incomingType, string input, MainPage link)
        {
            EventType = incomingType;
            EventContent = input;
            EventFinished = false;
            MainPage = link;
        }

        //Events ändern zur Laufzeit ihren EventType. 
        //      Bsp: Betti will Eric etwas schreiben.
        //1)    der TCP Listener erkennt eine eingehende Nachricht, er will nicht wissen was es ist, nur das es da ist.
        //      Er erstellt über den EventManager ein IncomingTCPMessage Event und übergibt ihm dabei die TCP-Nachricht.
        //2)    Main ruft zyklisch den EventManager.Run() auf, dieser Ruft Event.Run() und damit kommen wir hier her.
        //      Das Switch case findet heraus was für ein Event ansteht - in diesem Fall eine neue TCP Nachricht.
        //      In dieser Nachricht könnte alles mögliche drinn stehen, ein Login Versuch eines neuen Nutzers,
        //      Ein Broadcast, ein Kommando (bsp Für Serverneustart)... oder eine Private Nachricht wie in diesem Fall.
        //      Um herauszufinden wie mit der Nachricht umgegangen werden muss muss die Nachricht ANALysiert werden.
        //      Dazu wird TCPParser() aufgerufen.
        //3)    TCPParser zerlegt die Nachricht in seine Bestandteile. Er merkt dass es eine Private Nachricht ist welche 
        //      Von Betti kommt und zu Eric soll. Er ändert den EventTyp zu PrivateMessage.
        //4)    Event.Run() wird erneut durch den EventManager aufgerufen da EventFinished noch nicht true ist.
        //      Das Switch Case erkennt den EventTyp PrivateMessage und ruft den UserManager auf, gibt ihm einen String
        //      mit der Nachricht die gesendet werden soll, und Eric als Ziel an. Anschließend sendet der UserManager die Nachricht.
        //      Tritt hierbei kein Fehler auf wird
        //      das Event anschließend mit EventFinished = true als abgeschlossen markiert.
        //5)    Der EventManager erkennt dass EventFinished = true abgeschlossen ist und löscht das Event aus der Liste.

        public bool Run()
        {
            //HERE BE DRAGONS
            switch(EventType)
            {
                //Das Event kam vom TCP Manager. Es muss gelesen werden um zu verstehen wie wir darauf reagieren müssen.
                //Der Parser ändert anschließend den EventTyp und schreibt die Nutzdaten in einen Separaten String "Payload".
                case eventType.IncomingTCPMessage:
                    TCPParser();
                    break;

                //Try - Catch Blöcke lösen in Catch{} diesen EventTyp aus
                case eventType.DebugMessage:
                    //Das Event wird als Debuginfo ausgegeben und damit abgeschlossen.
                    //Der Clou an den meistens uninteressanten DebugInfos ist dass ich sie später per Knopfdruck ausblendbar machen will
                    PrintDebugMessageAsync("INFO: " + EventContent);
                    EventFinished = true;
                    break;

                case eventType.Error:
                    //Das Event wird als Fehlermeldung ausgegeben und damit abgeschlossen.
                    PrintDebugMessageAsync("ERROR: " + EventContent);
                    EventFinished = true;
                    break;

                    //Ein unbekanntes Event! Wie konnte das nur passieren!?
                default:
                    EventType = eventType.Error;
                    EventContent = "Unbekanntest Event aufgetreten! Sollte niemals passieren..." + EventContent;
                    break;

                    
            }
            return EventFinished;
        }

        private void TCPParser()
        {
            // die nachricht auf kommandozeichen und inhalte prüfen und separat abspeichern
            // die Eventklasse wird sich danach von TCPNachricht auf eine neue Klasse ändern


            //TCP String noch nicht definiert! Solang machen wir einfach aus allem DebugEvents.

            EventType = eventType.DebugMessage;
        }

        private async Task PrintDebugMessageAsync(string input)
        {
            //Schreibt zeugs in die TextBox auf der GUI.
            await MainPage.WriteToLogAsync(input);
        }

       
    }
}
