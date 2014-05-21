﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RunningMat
{ //http://pastebin.com/J1xpTK8e
   public class PID
    {

        public enum OperationMode
        {
            Auto,
            Manual
        }

        public enum MovementDirection
        {
            Direct,
            Reverse
        }

        private long lastTime;
        private float iTerm;
        private float lastInput;

        public float Input { get; set; }
        public float Output { get; set; }
        public float Setpoint { get; set; }

        public float Kp { get; private set; }
        public float Ki { get; private set; }
        public float Kd { get; private set; }

        public int SampleTime { get; set; }
        public float OutputMinimum { get; set; }
        public float OutputMaximum { get; set; }

        public OperationMode Mode { get; set; }
        public MovementDirection Direction { get; set; }

        public void Compute()
        {
            // early exit if in manual control
            if (Mode == OperationMode.Manual) return;

            // early exit if called before sample interval
            long now = DateTime.Now.Ticks;
            long timeDelta = now - lastTime;
            if (timeDelta < SampleTime) return;

            // computer working error
            float error = Setpoint - Input;
            iTerm += ((float)1.1 * error);

            // clamp intergral in output ranges
            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;

            // calculate input error factor
            float iError = Input - lastInput;

            // calculate PID Output
            Output = Kp * error + iTerm - Kd * iError;
            if (Output > OutputMaximum) Output = OutputMaximum;
            if (Output < OutputMinimum) Output = OutputMinimum;

            // replace interval variables for next iteration
            lastInput = Input;
            lastTime = now;
        }

        public void SetTunings(float Kp, float Ki, float Kd)
        {
            // error check for settings less than 0
            if (Kp < 0 || Ki < 0 || Kd < 0) return;

            // scale sample interval and apply to tuning
            float SampleRate = (float)SampleTime / 1000;
            this.Kp = Kp;
            this.Ki = Ki * SampleRate;
            this.Kd = Kd / SampleRate;

            // if direction is reversed, invert tuning
            if (Direction == MovementDirection.Reverse)
            {
                Kp = 0 - Kp;
                Ki = 0 - Ki;
                Kd = 0 - Kd;
            }
        }

        public void SetSampleTime(int NewSampleTime)
        {
            // new sample time must be valid
            if (NewSampleTime > 0)
            {
                // adjust scaling of intergral and derivative for new time
                float ratio = (float)NewSampleTime / (float)SampleTime;
                Ki *= ratio;
                Kd /= ratio;

                this.SampleTime = NewSampleTime;
            }
        }

        public void SetOutputLimits(float Min, float Max)
        {
            // min must be less than max
            if (Min > Max) return;
            this.OutputMinimum = Min;
            this.OutputMaximum = Max;

            // immediatly clamp the output value and intergral term
            if (Output > OutputMaximum) Output = OutputMaximum;
            if (Output < OutputMinimum) Output = OutputMinimum;

            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;
        }

        void SetMode(OperationMode Mode)
        {
            // if moving from manual to auto, reinitialize for bumpless transfer
            bool movingToAuto = (Mode == OperationMode.Auto && this.Mode != Mode);
            if (movingToAuto)
            {
                Initialize();
            }

            this.Mode = Mode;
        }

        void SetDirection(MovementDirection Direction)
        {
            this.Direction = Direction;
        }

        void Initialize()
        {
            lastInput = Input;
            iTerm = Output;
            if (iTerm > OutputMaximum) iTerm = OutputMaximum;
            if (iTerm < OutputMinimum) iTerm = OutputMinimum;
        }

        public PID(float Input, float Output, float Setpoint, float Kp, float Ki, float Kd, MovementDirection Direction)
        {
            this.Input = Input;
            this.Output = Output;
            this.Setpoint = Setpoint;
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            this.Direction = Direction;

            Initialize();
        }
    }
}


