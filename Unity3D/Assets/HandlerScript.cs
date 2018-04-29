using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class HandlerScript : MonoBehaviour {

	Thread receiveThread;
	UdpClient client;
	public int port;

	public string lastReceivedUDPPacket = "";
	public string allReceivedUDPPackets = "";

	public GameObject Ball;
	Vector3 up;
	bool jump;


	void Start () {
		init();
	}

	private void init(){

		up = new Vector3 (0f, 400f, 0f);
		jump = false;

		print ("UPDSend.init()");

		port = 5065;

		print ("Sending to 127.0.0.1 : " + port);

		receiveThread = new Thread (new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start ();

	}

	private void ReceiveData(){
		client = new UdpClient (port);
		while (true) {
			try{
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
				byte[] data = client.Receive(ref anyIP);

				string text = Encoding.UTF8.GetString(data);
				print (">> " + text);
				lastReceivedUDPPacket=text;
				allReceivedUDPPackets=allReceivedUDPPackets+text;

				jump = true;

			}catch(Exception e){
				print (e.ToString());
			}
		}
	}

	public void Jump(){
		Ball.GetComponent<Rigidbody> ().AddForce (up);
	}

	public string getLatestUDPPacket(){
		allReceivedUDPPackets = "";
		return lastReceivedUDPPacket;
	}

	// Update is called once per frame
	void Update () {
		if(jump == true){
			Jump ();
			jump = false;
		}

	}

	void OnApplicationQuit(){
		if (receiveThread != null) {
			receiveThread.Abort();
		}
	}
}
