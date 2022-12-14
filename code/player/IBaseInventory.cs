// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public interface IBaseInventory
{
	void OnChildAdded( Entity child );
	void OnChildRemoved( Entity child );
	void DeleteContents();
	int Count();
	Entity GetSlot( int i );
	int GetActiveSlot();
	bool SetActiveSlot( int i, bool allowempty );
	bool SwitchActiveSlot( int idelta, bool loop );
	Entity DropActive();
	bool Drop( Entity ent );
	Entity Active { get; }
	bool SetActive( Entity ent );
	bool Add( Entity ent, bool makeactive = false );
	bool Contains( Entity ent );
}
