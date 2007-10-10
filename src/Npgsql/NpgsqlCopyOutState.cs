// Npgsql.NpgsqlCopyOutState.cs
//
// Author:
// 	Kalle Hallivuori <kato@iki.fi>
//
//	Copyright (C) 2007 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//  Copyright (c) 2002-2007, The Npgsql Development Team
//  
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
// 
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.


using System;
using System.IO;

namespace Npgsql
{
    internal sealed class NpgsqlCopyOutState : NpgsqlState
    {
        private static NpgsqlCopyOutState _instance = null;

        private readonly String CLASSNAME = "NpgsqlCopyOutState";

        private NpgsqlCopyOutState() : base()
        { }

        public static NpgsqlCopyOutState Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = new NpgsqlCopyOutState();
                }
                return _instance;
            }
        }

        override protected void StartCopy( NpgsqlConnector context, NpgsqlCopyHeader copyHeader )
        {
            Stream userFeed = context.Mediator.CopyStream;
            if( userFeed == null )
            {
                context.Mediator.CopyStream = new NpgsqlCopyOutStream(context);
            }
            else
            {
              byte[] buf;
              while( (buf=GetCopyData(context)) != null )
                  userFeed.Write( buf, 0, buf.Length );
              userFeed.Close();
            }
        }

        override public byte[] GetCopyData( NpgsqlConnector context )
        {
            ProcessBackendResponses_Ver_3(context); // polling in COPY would take seconds on Windows
            return context.Mediator.ReceivedCopyData;
        }
    }
}
