#include <TFT_ILI9163.h>
#include <SPI.h>

class Timer
{
public:
  Timer();

  void start();
  void stop();
  void pause();
  void unpause();

  uint16_t getTicks();

  bool isStarted();
  bool isPaused();

private:
  uint16_t cStartTicks;
  uint16_t cPausedTicks;

  bool cPaused;
  bool cStarted;
};

TFT_ILI9163 tft = TFT_ILI9163(128, 128);
byte dataBuffer[16];
int cpuUsage, gpuUsage, cpuTemp, gpuTemp, memoryUsage, ramUsage, fps, powerUsage;
uint16_t colors[6] = {0x001F, 0x07E0, 0x07FF, 0xF800, 0xFFE0, 0xFFFF};

void printStaticText()
{
  randomSeed(analogRead(0));
  tft.fillScreen(ILI9163_BLACK);
  tft.setCursor(0, 0);
  tft.setTextSize(2);
  tft.setTextColor(colors[random(0, 6)]);
  tft.println("FPS: ");
  tft.println("GPU:     %");
  tft.println("CPU:     %");
  tft.println("RAM: ");
  tft.println("GRAM: ");
  tft.println("GPUT:    C");
  tft.println("CPUT:    C");
  tft.println("PWR:     W");
  tft.setTextColor(colors[random(0, 6)]);
}

void setup() {
  tft.init();
  tft.setRotation(2);
  tft.setTextWrap(false);
  printStaticText();

  Serial.begin(115200);
}

void loop() {
  Timer timer;
  timer.start();
  for(;;)
  {
    if(timer.getTicks() >= 60000)
    {
      printStaticText();
      timer.start();
    }
    if(Serial.available() >= 16)
    {
      Serial.readBytes(dataBuffer, 16);

      cpuUsage      = dataBuffer[0]   << 8 | dataBuffer[1];
      gpuUsage      = dataBuffer[2]   << 8 | dataBuffer[3];
      cpuTemp       = dataBuffer[4]   << 8 | dataBuffer[5];
      gpuTemp       = dataBuffer[6]   << 8 | dataBuffer[7];
      memoryUsage   = dataBuffer[8]   << 8 | dataBuffer[9];
      ramUsage      = dataBuffer[10]  << 8 | dataBuffer[11];
      fps           = dataBuffer[12]  << 8 | dataBuffer[13];
      powerUsage    = dataBuffer[14]  << 8 | dataBuffer[15];

      for(int index = 0; index < 8; index++)
      {
        int len;
        (index == 3 || index == 4) ? len = 5 : len = 4;
        for(int i = 0; i < len; i++)
        {
          tft.fillRect(60 + i * 12, 16 * index, 12, 16, ILI9163_BLACK);
        }
      }

      tft.setCursor(60, 0);
      tft.print(fps);

      tft.setCursor(60, 16);
      tft.print(gpuUsage);
      
      tft.setCursor(60, 32);
      tft.print(cpuUsage);

      tft.setCursor(60, 48);
      tft.print(ramUsage);

      tft.setCursor(60, 64);
      tft.print(memoryUsage);

      tft.setCursor(60, 80);
      tft.print(gpuTemp);

      tft.setCursor(60, 96);
      tft.print(cpuTemp);

      tft.setCursor(60, 112);
      tft.print(powerUsage);
    }
  }
}

Timer::Timer()
{
  cStartTicks = 0;
  cPausedTicks = 0;
  cStarted = false;
  cPaused = false;
}
void Timer::start()
{
  cStarted = true;
  cPaused = false;

  cStartTicks = millis();
  cPausedTicks = 0;
}
void Timer::stop()
{
  cStarted = false;
  cPaused = false;
  cStartTicks = 0;
  cPausedTicks = 0;
}
void Timer::pause()
{
  if (cStarted && !cPaused)
  {
    cPaused = true;
    cPausedTicks = millis() - cStartTicks;
    cStartTicks = 0;
  }
}
void Timer::unpause()
{
  if (cStarted && cPaused)
  {
    cPaused = false;
    cStartTicks = millis() - cPausedTicks;
    cPausedTicks = 0;
  }
}
uint16_t Timer::getTicks()
{
  uint16_t time = 0;
  if (cStarted)
  {
    if (cPaused)
      time = cPausedTicks;
    else
      time = millis() - cStartTicks;
  }
  return time;
}
bool Timer::isStarted()
{
  return cStarted;
}
bool Timer::isPaused()
{
  return cPaused && cStarted;
}
