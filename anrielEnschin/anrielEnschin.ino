

struct animation{
  unsigned long time;
  char port;
  char status;
};

struct message{
  unsigned long time;
  char message[20];
};

const int STARTUP = 1000;
const int SET_POWER = 1000; //min 40;
const char BRIGHTNESS = 30;
const int BUFFER_SIZE = 10;
const int ANIMATION_QUEUE_SIZE = 160;
const int MESSAGE_QUEUE_SIZE = 10;
const char SWITCHES[8] = {2,4,7,8,12,A2,A1,A0};
const char INVERT[8] = {1,1,1,1,0,1,1,1};
const char LEDS[6] = {11,10,6,9,5,3};

char buffer[BUFFER_SIZE+1];
int bufferFill;
animation animationQueue[ANIMATION_QUEUE_SIZE];
message messageQueue[MESSAGE_QUEUE_SIZE];
int animationFill = 0;
int messageFill = 0;
int power;
char led[8] = {0,0,0,0,0,0,0,0};
char switchStatus[8] = {0,0,0,0,0,0,0,0};
int consumption = 0;
char alive = false;
char blocked = false;

void setup() {
  power = 0;
  clearBuffer();
  Serial.begin(9600);// put your setup code here, to run once:
  pinMode(13, OUTPUT);
  for(int i = 0;i<8;i++){
  pinMode(SWITCHES[i],INPUT_PULLUP);
  }
  for(int i = 0;i<6;i++){
  pinMode(LEDS[i],OUTPUT);  
  analogWrite(LEDS[i],255);
  }
}



void loop() {
  //Unity to controller
  //S Request switch status -> SWITCHSTATUS|[0-1]|[0-1]|[0-1]|[0-1]|[0-1]|[0-1]|[0-1]|[0-1]
  //SWITCHSTATUS00000010
  //P[0-8] Set Power -> POWEROK
  //N[0-8] Start new Game -> READY Ready to start
  //O Gameover -> GAMEOVER Ready to Gameover
  //A[0-8] Play Animation number x, will be defined later

  //Controller to Unity
  //SWITCHSTATUS[0-1][0-1]|[0-1]|[0-1]|[0-1]|[0-1]|[0-1]|[0-1] Switch status
  //POK Setting Power successful
  //READY Ready to start
  //GAMEOVER Ready to Gameover
  //INVALID Invalid message
  
  consumption = 0;
  for(int i = 0;i<8;i++){
    char c = readSwitch(i);
    if(i!=7){
    consumption += c;
    }
    switchStatus[i] = c; 
  }
  if(alive&&power-consumption<0&&animationFill==0){
    unsigned long now = millis();
    blocked = true;
    fillUp(power,1500,now);
    fill(8,now+1500);
     fill(8,now+1700);
  }
  if(alive&&animationFill==0){
    fill(power-consumption,millis());  
  }

  for(int i = 0;i<messageFill;i++){
    if(messageQueue[i].time<millis()){
    Serial.println(messageQueue[i].message);
    if(removeFromMessageQueue(i)){
      i--;
    }
    }
  }
  
  for(int i = 0;i<animationFill;i++){
    if(animationQueue[i].time<millis()){
    analogWrite(animationQueue[i].port,255-animationQueue[i].status);
    if(removeFromAnimationQueue(i)){
      i--;
    }
    }
  }
  
  while (Serial.available() > 0) {
    char input = Serial.read();
    if(input=='\n'){
      executeCommand();
      clearBuffer();
    }else if(bufferFill<BUFFER_SIZE){
      buffer[bufferFill] = input;
      bufferFill++;
    }else{
      Serial.println("INVALID");
      clearBuffer();
    }
  }
  // put your main code here, to run repeatedly:

}

void executeCommand(){
  switch (buffer[0]){
    case 'S':
      requestStatus();
      break;
    case 'P':
      //entfernen
      buffer[1] = asciiToDec(buffer[1]);
      if(buffer[1]>=0&&buffer[1]<=8){
        setPower(buffer[1]);
      }else{
        Serial.println("INVALID");
      }
      break;
    case 'N':
      buffer[1] = asciiToDec(buffer[1]);
      if(buffer[1]>=0&&buffer[1]<=8){
        newGame(buffer[1]);
      }else{
        Serial.println("INVALID");
      }
      break;
    case 'O':
      gameover();
      break;
      
    case 'A':
      buffer[1] = asciiToDec(buffer[1]);
      if(buffer[1]>=0&&buffer[1]<=8){
        playAnimation(buffer[1]);
      }else{
        Serial.println("INVALID");
      }
      break;
    default:
      Serial.println("INVALID");
      break;
  }
}

void setPower(char mpower){
  power = mpower;
  if(!blocked){
  fillUp(power,SET_POWER*0.5,millis());
  fill(power,millis()+SET_POWER);
  }
}

void newGame(char mpower){
  alive = true;
  animationFill = 0;
  power = mpower;
  fillUp(power,STARTUP*0.6,millis());
  fill(power,millis()+SET_POWER);
}

void gameover(){
      unsigned long now = millis();
      animationFill = 0;
      fill(6,now);
      fill(0,now+10);
      fill(6,now+40);
      fill(0,now+50);
      fill(6,now+80);
      fill(0,now+90);
      fill(6,now+120);
      fill(0,now+130);
      fill(6,now+160);
      fill(0,now+170);
      alive = false;
}

void playAnimation(char ani){
  switch(ani){
    case 0:
      fill(0,millis());
      fill(0,millis()+30);
      break;
    case 1:
      fill(1,millis());
      fill(1,millis()+30);
      break;
  }
}

char readSwitch(int select){
  int c = digitalRead(SWITCHES[select]);
  switch (c){
    case HIGH:
     return 1-INVERT[select];
    case LOW:
      return 0+INVERT[select];
    }
}

void clearBuffer(){
  for(int i = 0;i<BUFFER_SIZE+1;i++){
    buffer[i]  = 0;
  }
  bufferFill = 0;
}

void requestStatus(){
  char status[21] = "SWITCHSTATUS";
  for(int i = 0;i<8;i++){
    char c = 'u';
    /*
    if(readSwitch(i)==0){
    c = '0';
    }else if(readSwitch(i)==1){
    
    c= '1';
    }
    */
    if(switchStatus[i]==0){
    c = '0';
    }else if(switchStatus[i]==1){
    
    c= '1';
    }
    status[11+i]=c;
  }
  status[20]=0;
  addToMessageQueue(status);
}

void addToMessageQueue(char* message){
  addToMessageQueue(message,millis());
}

void addToMessageQueue(char* message, unsigned long time){
  if(messageFill<MESSAGE_QUEUE_SIZE){
    strcpy(messageQueue[messageFill].message,message);
    messageQueue[messageFill].time = time;
    messageFill++;
  }else{
  //TODO MESSAGE QUEUE VOLL
  }
}

void addToAnimationQueue(char port, char status, unsigned long time){
  if(animationFill<ANIMATION_QUEUE_SIZE){
    animationQueue[animationFill].time = time;
    animationQueue[animationFill].port = port;
    animationQueue[animationFill].status = status;
    animationFill++;
  }else{
  //TODO MESSAGE QUEUE VOLL
  }
}

char removeFromMessageQueue(int pos){
  if(pos<=messageFill&&pos>=0){
  for(int i=pos;i<messageFill-1;i++){
    //Serial.println("move");
    //messageQueue[i].time = messageQueue[i+1].time;
    //strcpy(messageQueue[i].message,messageQueue[i+1].message);
    messageQueue[i] = messageQueue[i+1];
  }
  messageFill--;
  return 1;
  }else{
  return 0;
  }
}


char removeFromAnimationQueue(int pos){
  if(pos<=animationFill&&pos>=0){
  for(int i=pos;i<animationFill-1;i++){
    //Serial.println("move");
    //messageQueue[i].time = messageQueue[i+1].time;
    //strcpy(messageQueue[i].message,messageQueue[i+1].message);
    animationQueue[i] = animationQueue[i+1];
  }
  animationFill--;
  return 1;
  }else{
  return 0;
  }
}

char asciiToDec(char ascii) {
  switch(ascii){
        case '0':
        return 0;
      case '1':
        return 1;
      case '2':
        return 2;
      case '3':
        return 3;
      case '4':
        return 4;
      case '5':
        return 5;
      case '6':
        return 6;
      case '7':
        return 7;
      case '8':
        return 8;
      case '9':
        return 9;
      default:
        return -1;  
  }
}


void fillUp(int amount,unsigned long duration,unsigned long time){
  unsigned long div = duration/(amount-1);
  fill(0,time);
  for(int i = 1;i<amount;i++){
    fill(i,time+(div*i));
  }
}

void fill(int amount,unsigned long time){
  for(int i = 0;i<amount;i++){
    addToAnimationQueue(LEDS[i],BRIGHTNESS,time);
  }
  for(int i = amount;i<8;i++){
    addToAnimationQueue(LEDS[i],0,time);
  }
}
