#include <Arduino.h>
#include <FastLED.h>

#define LEVER 0
#define LED_PIN 12

CRGB realLights[6];
CRGB lights[6];

void setup() {
    for (int i = 2; i < 12; i++) {
        pinMode(i, INPUT_PULLUP);
    }

    Serial.begin(38400);

    delay(3000);
    FastLED.addLeds<WS2812B, LED_PIN, GRB>(realLights, 6);
}

void updateLight(int pin, int i) {
    realLights[i] = lights[i];
    if (digitalRead(pin) == LOW) {
        realLights[i].r = min(255, realLights[i].r + 25);
        realLights[i].g = min(255, realLights[i].g + 25);
        realLights[i].b = min(255, realLights[i].b + 25);
    }
}

void loop() {
    if (Serial.available() > 0) {
        int cmd = Serial.read();
        switch (cmd) {
            case 'D': {
                Serial.print("{\"right\":[");
                for (unsigned char i = 2; i < 7; i++) {
                    if (digitalRead(i) == LOW) {
                        Serial.print(1);
                    } else {
                        Serial.print(0);
                    }

                    if (i != 6) Serial.print(',');
                }

                Serial.print("], \"left\":[");

                for (int i = 7; i < 12; i++) {
                    if (digitalRead(i) == LOW) {
                        Serial.print(1);
                    } else {
                        Serial.print(0);
                    }

                    if (i != 11) Serial.print(',');
                }

                Serial.print("], \"lever\":");
                Serial.print(analogRead(LEVER));
                Serial.print("}\n");
                break;
            }
            case 'L': {
                int b = Serial.read();
                FastLED.setBrightness(b);
                for (auto &light: lights) {
                    int b = Serial.read();
                    int g = Serial.read();
                    int r = Serial.read();
                    light = CRGB(b, g, r);
                }
                break;
            }
        }
    }

    for (int i = 0; i < 3; i++) {
        updateLight(i + 7, (2 - i));
        updateLight(i + 2, (5 - i));
    }

    FastLED.show();
}
