#!/usr/local/bin/perl

print ("Copying in UnitySteer...\n");
system ("cp -R Source/UnitySteer Test\\ project/Assets");
system ("rm -rf Test\\ project/Assets/UnitySteer/.git");
print ("Done!\n");