using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Windows.Networking.Sockets;
using System.IO;

namespace RaspberryServerApp
{

    class TCPConnection
    {
        private EventManager EventManager;
        private static StreamSocketListener socketListener;



        //Init() erstellt einen Socket. Ein Socket ist eine Verbindung zwischen IP-Adresse und Port. Also eine eindeutige Verbindungsinformation
        //In diesem Fall habe ich mich für einen Asynchronen Socket entschieden. Dieser wird nur einmalig Initialisiert und nie wieder angefasst.
        async public Task Init(EventManager link)
        {
            EventManager = link;
            EventManager.CreateEvent(eventType.DebugMessage, "Listener wird vorbereitet...");
            try
            {
                socketListener = new StreamSocketListener();

                //Das hier ist der Trick der Asynchronen Programmierung. Ich habe einen Socket erstellt und mittels += Operator ein Event an eine Methode gekoppelt.
                //Jedes Mal wenn das Event "ConnectionReceived" auftritt wird automatisch die Methode ConnectionReceivedHandlerAsync aufgerufen.
                //Wenn nie eine Nachricht ankommen würde würde auch im Code nie etwas passieren. Es wird auf ein Event gewartete anstatt die ganze Zeit aktiv abzufragen ob
                //eine Verbindung da ist. Das spart Strom und CPU Laufzeit. Sieht dafür verwirrender für uns aus.
                socketListener.ConnectionReceived += ConnectionReceivedHandlerAsync;
                //KeepAlive = true bedeutet dass der Listener nie ausgehen soll. Stürtzt er ab startet er automatisch neu. Wir müssen uns um nix kümmern.
                socketListener.Control.KeepAlive = true;
                await socketListener.BindServiceNameAsync("9000");
            }
            catch (Exception e)
            {
                EventManager.CreateEvent(eventType.Error, e.Message);
            }

        }


        //Wenn der Socket ein ConnectionReceived Event feuert kommen wir hier an. In genau diesem Moment haben wir eine Direkte Verbindung zu einem Client.
        //Diese Verbindung, der Stream, kann jetzt beidseitig mit Daten geflutet werden. Wir wollen momentan nur empfangen, deshalb gibt es nur einen
        //StreamReader. Er schreibt solange der Stream besteht daten in ein den input String. Natürlich ist auch diese Funktion wieder Asynchron, wir wissen ja nicht
        //wie lange der Client Daten senden will und wie viele das sind. Vor ReadLineAsync steht deshalb AWAIT. Das bewirkt dass auf eine Asynchrone Funktion welche
        //normalerweise im Hintergrund weiterläuft und irgendwann mal fertig ist gewartet wird. Ansonsten würde die Methode einfach nach dem Aufruf von ReadLineAsync
        //weiterrennen. Im input String wäre dann einfach nichts.
        private async void ConnectionReceivedHandlerAsync(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            string input;

            EventManager.CreateEvent(eventType.DebugMessage, "Neue TCP Verbindung: " + args.Socket.Information.RemoteAddress);
            try
            {
                Stream inStream = args.Socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(inStream);
                input = await reader.ReadLineAsync();
                //Die gelesenen Daten im String werden in ein IncomingTCPMessage Event gekippt.
                EventManager.CreateEvent(eventType.IncomingTCPMessage, input);

            }
            catch (Exception e)
            {
                EventManager.CreateEvent(eventType.Error, e.Message);
            }

        }

        public void Run()
        {

        }
    }
}
