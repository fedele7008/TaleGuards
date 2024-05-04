# TaleGuards

TalesGuards is multi-platform turn-based RPG game that provides both
PVE and PVP contents.

We are going to follow propagated single-sign-in (SSO) model for the
project architecture.

The `authentication server` will be aliased `Akashic` in this project -
related to the game content.

Note that `Akashic` and `TaleGuards Server` will be separated solution
for future AWS deployment. The separation will prevent coupling between
two server instances.
 