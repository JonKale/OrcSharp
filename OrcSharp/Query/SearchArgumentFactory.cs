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

namespace OrcSharp.Query
{
    using System;

    /**
     * A factory for creating SearchArguments, as well as modifying those created by this factory.
     */
    public class SearchArgumentFactory
    {
        public static SearchArgument.Builder newBuilder()
        {
            return new SearchArgumentImpl.BuilderImpl();
        }
        public static void setPredicateLeafColumn(PredicateLeaf leaf, string newName)
        {
            SearchArgumentImpl.PredicateLeafImpl.setColumnName(leaf, newName);
        }
    }
}
