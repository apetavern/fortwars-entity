using Sandbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fortwars
{
	public static class Profiler
	{
		private static Stopwatch sw = Stopwatch.StartNew();
		private static Entry Root = new Entry();
		private static TimeSince timeSince;

		internal class Entry
		{
			public string Name { get; set; }
			public float TotalTime { get; private set; }

			private int callCount;
			private List<Entry> Children;

			public Entry GetOrCreateChild( string name )
			{
				Children ??= new();

				for ( int i = 0; i < Children.Count; i++ )
				{
					if ( Children[i].Name == name )
						return Children[i];
				}

				var e = new Entry();
				e.Name = name;

				Children.Add( e );
				return e;
			}

			public void Add( float v )
			{
				callCount++;
				TotalTime += v;
			}

			public void Clear()
			{
				callCount = 0;
				TotalTime = 0;

				if ( Children == null ) return;

				for ( int i = 0; i < Children.Count; i++ )
				{
					Children[i].Clear();
				}
			}

			public string GetString( int indent = 0 )
			{
				var str = $"{new string( ' ', indent * 4 )}{TotalTime:0.00}ms  {callCount} - {Name}\n";

				if ( indent == 0 )
					str = "";

				if ( Children == null )
					return str;

				foreach ( var child in Children.OrderByDescending( x => x.TotalTime ) )
				{
					if ( child.callCount == 0 ) continue;
					str += child.GetString( indent + 1 );
				}

				return str;
			}
		}

		public static IDisposable CreateScope( string name ) => new ProfileScope( name );

		[Event.Hotload]
		private static void OnHotload()
		{
			Root = new Entry();
		}

		[Event.Tick]
		private static void OnFrame()
		{
			if ( timeSince >= 5.0f )
			{
				timeSince = 0;

				DebugOverlay.ScreenText( 20, Root.GetString(), 5.0f );
			}

			Root.Clear();
		}

		internal struct ProfileScope : System.IDisposable
		{
			Entry parentEntry;
			Entry entry;

			public double StartTime { get; set; }

			public ProfileScope( string name )
			{
				parentEntry = Profiler.Root;

				entry = parentEntry.GetOrCreateChild( name );
				StartTime = sw.Elapsed.TotalMilliseconds;
				Profiler.Root = entry;
			}

			public void Dispose()
			{
				entry.Add( (float)(sw.Elapsed.TotalMilliseconds - StartTime) );
				Profiler.Root = parentEntry;

				Log.Trace( $"{entry.Name} took {entry.TotalTime.CeilToInt()}ms" );
			}
		}
	}
}
