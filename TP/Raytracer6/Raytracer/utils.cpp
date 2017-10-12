#include "stdafx.h"
#include "vec3.h"
#include "utils.h"
#include <fstream>

double clamp(double valeur, double min, double max) {
	if (valeur < min) {
		return min;
	}
	else{
		if (valeur > max) {
			return max;
		}
		else
		{
			return valeur;
		}
	}
}

vec3 clamp(vec3 valeur, double min, double max)
{
	return vec3(clamp(valeur.getX(), min, max), clamp(valeur.getY(), min, max), clamp(valeur.getZ(), min, max));
}

void save_img(std::string nom, int w, int h, pixel ** couleurs) {
	std::fstream writer;
	writer.open(nom+".ppm", std::fstream::out);
	writer << "P3\n";
	writer << w << " " << h << "\n";
	writer << "255\n";

	for (int i = h-1; i >= 0; i--) {
		for (int j = 0; j < w; j++) {
			writer << clamp( couleurs[i][j].getR(), 0, 255 ) << " " << clamp(couleurs[i][j].getG(), 0, 255) << " " << clamp(couleurs[i][j].getB(), 0, 255) << "\n";
		}
	}

	writer.close();
}

// Comparaison entre deux valeurs du type double (precision de 4 chifres)
bool equalsDouble(double value1, double value2) {
	return (value1 - value2 < 0.0001) && (value2 - value1 < 0.0001);
}

double couleurToAlbedo( double couleur ) {
	return couleur / 255.;
}
vec3 couleurToAlbedo(vec3 couleur) {
	return vec3( couleurToAlbedo(couleur.getX()), couleurToAlbedo(couleur.getY()), couleurToAlbedo(couleur.getZ()));
}
double albedoToCouleur(double albedo) {
	return albedo * 255.;
}
vec3 albedoToCouleur(vec3 albedo) {
	return vec3( albedoToCouleur(albedo.getX()), albedoToCouleur(albedo.getY()), albedoToCouleur(albedo.getZ()) );
}

vec3 clampCouleur(vec3 couleur)
{
	return clamp(couleur,0,255);
}
