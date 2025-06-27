This project started as a marching cubes project. there is a working marching cube script in the project, however it only creates a small chunk of marched cubes.
I spent a while trying to figure out compute shaders to allow for the marching cubes to work faster however i soon dropped that and started looking into trying to recreate
the smoke mechanics from CS using voxels. In the projects current state, there is a test scene that allows you to move a "grenade" placement object and click the deploy button in the inspector
and it will create a voxel sphere around the smoke. The creation of the smoke it on a lerp so it happens over time and slows down towards the end.
Using the voxel grid the smoke will only deploy in empty voxels and will curve around objectsalso filling spaces that its in. To prevent voxels being spawned on the other side of thin walls
there is a floodfill algorithm to check which voxels are in reach of the source.


https://github.com/user-attachments/assets/5bffbace-0842-44a9-b1ff-dd0b7242bf06

