//-----------------------------------------------------------------------------
// Copyright (c) 2013 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

function AquariumToy::create( %this )
{
    exec("./scripts/aquarium.cs");

    // Configure settings.
    AquariumToy.createFishScheduleId = "";
    AquariumToy.maxFish = 10;
    AquariumToy.currentFish = 0;
    AquariumToy.selectedAnimation = "AquariumToy:angelfish1Anim";
    
    // Set all the fish scene-layers to sort in "batch" mode
    // so that all the fish will be sorted into a batchable order to reduce draw calls.
    SandboxScene.setLayerSortMode( 0, batch );
    SandboxScene.setLayerSortMode( 1, batch );
    SandboxScene.setLayerSortMode( 2, batch );
    SandboxScene.setLayerSortMode( 3, batch );
    SandboxScene.setLayerSortMode( 4, batch );
    SandboxScene.setLayerSortMode( 5, batch );
    
    addNumericOption("Max Fish", 1, 50, 1, "setMaxFish", %this.maxFish, true, "Sets the maximum number of fish to be created.");
    addSelectionOption(getFishAnimationList(), "Fish Animation", 5, "setSelectedAnimation", false, "Selects the fish animation that can be spawned manually.");
    addButtonOption("Spawn fish", "spawnOneFish", false, "Spawns the selected fish animation." );  

    // Reset the toy initially.
    AquariumToy.reset();
}

//-----------------------------------------------------------------------------

function AquariumToy::destroy( %this )
{
    // Cancel any pending events.
    AquariumToy::cancelPendingEvents();
}

//-----------------------------------------------------------------------------

function AquariumToy::reset(%this)
{
    // Clear the scene.
    SandboxScene.clear();

    // Set the gravity.
    SandboxScene.setGravity(0, 0);

    buildAquarium();
    createAquariumEffects();

    // Reset the ball count.
    %this.currentFish = 0;

    // Cancel any pending events.
    AquariumToy::cancelPendingEvents();

    // Schedule to create a ball.
    %this.createFishScheduleId = %this.schedule( 100, "spawnFish" );
}

//-----------------------------------------------------------------------------

function AquariumToy::cancelPendingEvents(%this)
{
    // Finish if there are not pending events.
    if ( !isEventPending(%this.createFishScheduleId) )
        return;

    // Cancel it.
    cancel(%this.createFishScheduleId);
    %this.createFishScheduleId = "";
}

//-----------------------------------------------------------------------------

function AquariumToy::spawnFish(%this)
{
    // Reset the event schedule.
    %this.createFishScheduleId = "";

    %position = getRandom(-55, 55) SPC getRandom(-20, 20);
    %index = getRandom(0, 5);
    %anim = getUnit(getFishAnimationList(), %index, ",");

    %fishInfo = getFishSize(%anim);

    %fish = new Sprite()
    {
        Animation = %anim;
        class = "FishClass";
        position = %position;
        size = %fishInfo;
        SceneLayer = "2";
        SceneGroup = "14";
        minSpeed = "5";
        maxSpeed = "15";
        CollisionCallback = true;
    };

    %fish.createPolygonBoxCollisionShape( 15, 15);
    %fish.setCollisionGroups( 15 );
    %fish.setDefaultDensity( 1 );
    %fish.setDefaultFriction( 1.0 );
    SandboxScene.add( %fish );

    %this.currentFish++;

    // Schedule to spawn a fish.
    if ( %this.currentFish < %this.maxFish)
        %this.createFishScheduleId = %this.schedule( 100, "spawnFish" );
}

//-----------------------------------------------------------------------------

function AquariumToy::setMaxFish(%this, %value)
{
    %this.maxFish = %value;
}
//-----------------------------------------------------------------------------

function AquariumToy::setSelectedAnimation(%this, %value)
{
    %this.selectedAnimation = %value;
}

//-----------------------------------------------------------------------------

function AquariumToy::spawnOneFish(%this)
{
    %position = getRandom(-55, 55) SPC getRandom(-20, 20);

    %fishSize = getFishSize(%this.selectedAnimation);

    %fish = new Sprite()
    {
        Animation = %this.selectedAnimation;
        class = "FishClass";
        position = %position;
        size = %fishSize;
        SceneLayer = "2";
        SceneGroup = "14";
        minSpeed = "5";
        maxSpeed = "15";
        CollisionCallback = true;
    };

    %fish.createPolygonBoxCollisionShape( 15, 15);
    %fish.setCollisionGroups( 15 );
    %fish.setDefaultDensity( 1 );
    %fish.setDefaultFriction( 1.0 );
    SandboxScene.add( %fish );
}


