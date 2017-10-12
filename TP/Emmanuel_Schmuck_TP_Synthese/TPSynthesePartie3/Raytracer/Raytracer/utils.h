#pragma once



#include <iostream>
#include <iomanip>
#include <string>
#include <fstream>
#include "vec.h"
#include "ray.h"
#include "sphere.h"
#include "scene.h"



// clamp a float value between min and max
float clamp(float value, float min, float max) {

	if (value < min) { return min; }
	else if (value > max) { return max; }
	else { return value; }
}

// lerp (linear interpolation) between two floats
float lerp(float a, float b, float alpha) {
	alpha = clamp(alpha, 0, 1);
	return (1 - alpha) * a + alpha * b;
}

// save a ppm image file from the rgb value in a pixel array
// filename : name of the file, ending with .ppm (string)
// pixels : a vec3 array of pixels (r,g,b)
// width : image width (int)
// height : image height (int)
void save_img(string filename, vec3* pixels, int width, int height) {

	int IMG_WIDTH = width;
	int IMG_HEIGHT = height;
	ofstream myfile;
	
	myfile.open(filename);
	myfile << "P3\n" << IMG_WIDTH <<" "<<IMG_HEIGHT<<"\n" << "255 \n";

	float r, g, b;

	cout << "writing in file : " <<filename<< endl;
	
	for (int i = 0; i < IMG_HEIGHT; i++) {
		
		cout << "\r" << setprecision(3) << 100 * (float)i / IMG_HEIGHT << " %      " ;
		for (int j = 0; j < IMG_WIDTH; j++) {

			r = clamp(pixels[i*IMG_WIDTH + j].getX(), 0, 255);
			g = clamp(pixels[i*IMG_WIDTH + j].getY(), 0, 255);
			b = clamp(pixels[i*IMG_WIDTH + j].getZ(), 0, 255);
			myfile <<r<<" "<<g<<" "<<b<<"\n";		
		}
	}
	myfile.close();
}