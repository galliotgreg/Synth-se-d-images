#include <string>
#include "pixel.h"

#pragma once
double clamp(double valeur, double min, double max);
vec3 clamp(vec3 valeur, double min, double max);

void save_img(std::string nom, int w, int h, pixel ** couleurs);

// Comparaison entre deux valeurs du type double (precision de 4 chifres)
bool equalsDouble(double value1, double value2);

double couleurToAlbedo(double couleur);
vec3 couleurToAlbedo(vec3 couleur);
double albedoToCouleur(double albedo);
vec3 albedoToCouleur(vec3 albedo);
vec3 clampCouleur(vec3 couleur);