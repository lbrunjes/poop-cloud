/*
A sketch to run a baby details thing.
Lee Brunjes


THE TERRIBLE DIAGRAM

        +V
        |       
+--+-+-+-+----+
|  | | | |  TOGGLE
=SWITCHES=   |
|  | | | |   |
PINS TO READ

& WIFI


*/
#include <WiFi.h>

//# of loops to wait to enable dhte button again.
int BUTTON_WAIT = 5000;
int TX_LED = 0;
int WAIT_LED = 1;

//set up buttons to monitor
int disabled  = 0;
int BUTTONS[] { 2,3,4,5,6,7};
const int BUTTONS_LENGTH = 6;
char DETAILS[BUTTONS_LENGTH][12] = {"bottle", "l-to-r", "r-to-l","urine", "feces", "mixed"};
char TYPES[BUTTONS_LENGTH][12] = {"feed", "feed", "feed","diaper", "diaper", "diaper"};

//configure sleep switch
int SLEEP_SWITCH = 8;
char SLEEP[] ="sleep";
char SLEEP_DETAILS[2][6] ={"awoke","slept"};
int ssLast;



//setup wifi network for WPA-PERSONAL
int use_wifi =0;
char SSID[]="Unassuming";
char PASSWORD[]= "zombiebrains";

WiFiClient internet;

//API CONFIG
char API_HOST[] = "tyr";
int  API_PORT   = 80;
char API_USER[] = "admin";
char API_PASS[] = "admin123";
char API_PATH[] = "/Service.ashx?";




void setup(){
  
  //init Serial
  Serial.begin(9600);
  //startwifi and disabe on failure
  if(use_wifi >0){
    int status = WiFi.begin(SSID,PASSWORD);
    if(status != WL_CONNECTED){
       Serial.println("NO WIFI CONNECTION");
      use_wifi = 0; 
    }
  }

  //setup the leds
  pinMode(TX_LED, OUTPUT);
  pinMode(WAIT_LED, OUTPUT);
  
  //setup the buttons
  for(int i = 0; i < BUTTONS_LENGTH;i++){
    pinMode(BUTTONS[i], INPUT);
  }
  
  //init sleep switch
  pinMode(SLEEP_SWITCH, INPUT);
  ssLast= digitalRead(SLEEP_SWITCH);
  
}

void loop(){

  //did we turn off input as anti spam
  if(disabled >0){
    disabled --;
  } 
  else{
    //set wait led off.
    digitalWrite(WAIT_LED, LOW);
    
    //check to find button press
    for(int i = 0; i < BUTTONS_LENGTH;i++){
         if(digitalRead(BUTTONS[i]) == HIGH){
           SendMessage(TYPES[i], DETAILS[i]);
         }
    }
    //check to see if the switch changed places for sleep
    if(digitalRead(SLEEP_SWITCH) != ssLast){
      if(ssLast == HIGH){
        SendMessage(SLEEP, SLEEP_DETAILS[0]);
      }
      else{
         SendMessage(SLEEP,SLEEP_DETAILS[1]);
      }
    }
  }
  
  
}

// this is how we actaullly do communications  defautl to serial and use wife if avaialbe
void SendMessage( char* type, char* detail ){
  
  digitalWrite(WAIT_LED, HIGH);
  disabled = BUTTON_WAIT;
  
  
  if(use_wifi >0){
      digitalWrite(TX_LED, HIGH);
      
      internet.stop();
      //if(internet.connect(user + ":"+ pass +"@" + server))
      //TODO user http base 
      /*
      post
      http://user:paassword@host?type=type&detail=detail HTTP/1.1
      */
      
      digitalWrite(TX_LED, LOW);
  }
}

