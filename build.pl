#!/usr/local/bin/perl

use Term::ANSIColor;

our $compiler = "/Applications/Unity/Unity.app/Contents/Frameworks/Mono/bin/gmcs";
our $assemblyUnityEngine = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll";
our $assemblyUnityEditor = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll";

print ("Building runtime assembly...\n");
BuildAssembly ("library", "Path.Runtime.dll", "Source/*.cs", "-d:RUNTIME -r:$assemblyUnityEngine");
BuildAssembly ("library", "Path.Editor.dll", "Source/PathInspector.cs", "-d:EDITOR -r:Path.Runtime.dll,$assemblyUnityEngine,$assemblyUnityEditor");
system ("cp Path.Runtime.dll Test\\ project/Assets/Path");
system ("cp Path.Editor.dll Test\\ project/Assets/Path/Editor");

#print ("Copying in UnitySteer...\n");
#system ("cp -R Source/UnitySteer Test\\ project/Assets");
#system ("rm -rf Test\\ project/Assets/UnitySteer/.git");
print ("Done!\n");


sub BuildAssembly
{
	our $compiler;
	
	my $target = shift;
	my $out = shift;
	my $arguments = shift;
	my $source = shift;
	
	print ("Compiling $out\n");
	print color ("blue"), "$compiler -target:$target -out:$out $arguments $source\n";
	print color ("red");
	system ("$compiler -target:$target -out:$out $arguments $source") and die ("Compilation of $out failed.");
	print color ("reset");
}