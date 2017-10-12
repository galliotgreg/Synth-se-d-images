// TP Lancer de Rayon, Partie 3


// all other includes are in utils.h
#include "utils.h"
#include "Test.h";

// 3 colors
const vec3 fuschia(181, 51, 137);
const vec3 blue(31, 117, 254);
const vec3 green(28, 172, 120);
const vec3 yellow(253, 255, 0);
const vec3 white(255, 255, 255);



int main() {

	cout << endl << "TP Synthese d'Image" << endl << endl;

	// uncomment this to test functionalities
	//makeTest();

	cout << endl << "===== spheres : image generation start ... ===== " << endl << endl;

	const int width = 1024;
	const int height = 1024;

	// camera fov
	const float fov = 60 * 3.1415 / 180;

	// fog color
	const vec3 fogColor(20,20,60);

	// fog max opacity
	const float fogMaxOpacity = 1.00;

	// fog distance (fog will reach max opacity at this distance)
	const float fogDist = 170;

	// fog exponent (power law)
	const float fogExponent = 1.35;

	const vec3 cameraPosition(0, 10, 20);


	// array of vec3 with all the pixels
	vec3* pixels = new vec3[width * height];

	// camera position and max amount of spheres
	scene myScene(cameraPosition, 50);

	// 7 spheres
	myScene.addSphere(vec3(0, 10, -50), 10, yellow);
	myScene.addSphere(vec3(0, 40, -50), 10, fuschia);
	myScene.addSphere(vec3(25, -15, -50), 10, green);
	myScene.addSphere(vec3(-25, -15, -50), 10, blue);
	myScene.addSphere(vec3(0, 40, -110), 10, fuschia);
	myScene.addSphere(vec3(25, -15, -110), 10, green);
	myScene.addSphere(vec3(-25, -15, -110), 10, blue);

	// "walls"
	myScene.addSphere(vec3(0, -2000-20, 0), 2000, vec3(200, 200, 200));
	myScene.addSphere(vec3(0, 2000 +50, 0), 2000, vec3(200, 200, 200));
	myScene.addSphere(vec3(-2000-50, 0, 0), 2000, vec3(225, 225, 225));
	myScene.addSphere(vec3(2000+50, 0, 0), 2000, vec3(225, 225, 225));
	myScene.addSphere(vec3(0, 0, -2000-200), 2000, vec3(250, 250, 250));

	// setup
	float x, y, z, opacity;
	float t = 0;
	ray _ray;
	vec3 objectColor, finalColor;

	cout << "calculating pixel values..." << endl;

	// calculating the rgb value for each pixel
	for (int i = 0; i < height; i++) {

		// display progress
		cout << "\r" << setprecision(3) << 100 * (float)i / height << " %      ";

		for (int j = 0; j < width; j++) {

			// xyz for ray direction
			x = j - width / 2;
			y = - i + height / 2;
			z = -width / (2 * tan(fov / 2));

			_ray.set(myScene.getCameraPosition(), vec3(x, y, z));

			// if we hit a scene object (sphere)
			if (myScene.intersect(_ray,t, objectColor)) {
				finalColor = objectColor;

				// blend objects with power-law fog
				opacity = lerp(0, fogMaxOpacity, pow(t / fogDist, fogExponent));
				finalColor = vec3::lerp(finalColor, fogColor, opacity);
			}
			else {	
				finalColor.set(0, 0, 0);
			}
			
			// fill pixel array with final rgb values
			pixels[i*width + j] = finalColor;
		}
	}

	cout << endl << "saving image to ppm file..." << endl;

	save_img("spheres.ppm", pixels, width, height);

	cout << endl << endl << "===== end of image generation ===== " << endl << endl;

	system("pause");

	delete[] pixels;

	return 0;

}