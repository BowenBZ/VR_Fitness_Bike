# VRBike-Display

This repositoriy is used for the VR display part of project Cyclotron. 

## Insturction
This project is a simulated bike game. The speed of the bike and the turning can be controled external signal. The game can also be controled by keyboard or game handle(switch handle).

The control method and type are as follows.
* Go forward: `W` / left Y axis in handle
* Turn left: `A` / left X axis in handle
* Turn right: `D` / right X axis in handle
* Increase the speed: Up arrow / button `A` in handle
* Decrease the speed: Down arrow / button `B` in handle
* Switch to first person view: `F1` / button `R2` in handle
* Switch to thrid person view: `F2` / button `R2` in handle
* Restart: Right `Shift` + `R` / button `Y` in handle

When the bike go forward, the speed is fixed, whatever terrain you are in.

## Running Environment
* Unity 2018.3.12f1
* Visual Studio 2017

## GameObject and Script Overview
* Direct Light
* Manager
	* DuplicateRoads.cs: Generate road according to parameters
	* DuplicateTerrains.cs: Duplicate terrains according to the parameters
	* KeyboardControls.cs: Detect the input
	* ControlHub.cs: Contains all the variable related to the movement of bike
* Road: Unit of road
* Terrain: Unit of terrain
* Bike
	* Bicycle_code.cs: Control the movement of the bike
	* Bicycle_body
		* char_anim
			* Biker_logic_mecanim.cs: Control the action of the character
		* bicycle_pedals
			* PedalControls.cs: Control the action of pedals
	* FirstView: First view camera 
* CamBike
	* CamSwitcher.cs: Control different cameras enability
	* Back Camera: Third person view camera
	* Camera: A camera controled by right click of mouse

## Adopted Assets
* bicycle_pro_kit
* Mapbox
* 