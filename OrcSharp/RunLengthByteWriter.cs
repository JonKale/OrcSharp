/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace OrcSharp
{
    /// <summary>
    /// A streamFactory that writes a sequence of bytes. A control byte is written before
    /// each run with positive values 0 to 127 meaning 2 to 129 repetitions. If the
    /// bytes is -1 to -128, 1 to 128 literal byte values follow.
    /// </summary>
    class RunLengthByteWriter
    {
        public const int MIN_REPEAT_SIZE = 3;
        public const int MAX_LITERAL_SIZE = 128;
        public const int MAX_REPEAT_SIZE = 127 + MIN_REPEAT_SIZE;
        private PositionedOutputStream output;
        private byte[] literals = new byte[MAX_LITERAL_SIZE];
        private int numLiterals = 0;
        private bool repeat = false;
        private int tailRunLength = 0;

        public RunLengthByteWriter(PositionedOutputStream output)
        {
            this.output = output;
        }

        private void writeValues()
        {
            if (numLiterals != 0)
            {
                if (repeat)
                {
                    output.WriteByte((byte)(numLiterals - MIN_REPEAT_SIZE));
                    output.Write(literals, 0, 1);
                }
                else
                {
                    output.WriteByte((byte)-numLiterals);
                    output.Write(literals, 0, numLiterals);
                }
                repeat = false;
                tailRunLength = 0;
                numLiterals = 0;
            }
        }

        public void flush()
        {
            writeValues();
            output.Flush();
        }

        public void write(byte value)
        {
            if (numLiterals == 0)
            {
                literals[numLiterals++] = value;
                tailRunLength = 1;
            }
            else if (repeat)
            {
                if (value == literals[0])
                {
                    numLiterals += 1;
                    if (numLiterals == MAX_REPEAT_SIZE)
                    {
                        writeValues();
                    }
                }
                else
                {
                    writeValues();
                    literals[numLiterals++] = value;
                    tailRunLength = 1;
                }
            }
            else
            {
                if (value == literals[numLiterals - 1])
                {
                    tailRunLength += 1;
                }
                else
                {
                    tailRunLength = 1;
                }
                if (tailRunLength == MIN_REPEAT_SIZE)
                {
                    if (numLiterals + 1 == MIN_REPEAT_SIZE)
                    {
                        repeat = true;
                        numLiterals += 1;
                    }
                    else
                    {
                        numLiterals -= MIN_REPEAT_SIZE - 1;
                        writeValues();
                        literals[0] = value;
                        repeat = true;
                        numLiterals = MIN_REPEAT_SIZE;
                    }
                }
                else
                {
                    literals[numLiterals++] = value;
                    if (numLiterals == MAX_LITERAL_SIZE)
                    {
                        writeValues();
                    }
                }
            }
        }

        public void getPosition(PositionRecorder recorder)
        {
            output.getPosition(recorder);
            recorder.addPosition(numLiterals);
        }
    }
}
