#include "stdafx.h"
#include "lumiere.h"

#include "stdafx.h"
#include "sphere.h"
#include <math.h>
#include <algorithm>
#include <iostream>
using namespace std;

lumiere::lumiere(vec3 centre, vec3 couleur, double intensite)
{
	this->set(centre, couleur, intensite);
}

void lumiere::set(vec3 centre, vec3 couleur, double intensite) {
	this->setCentre(centre);
	this->setCouleur(couleur);
	this->setIntensite(intensite);
}
void lumiere::setCouleur(vec3 couleur) {
	this->couleur = couleur;
}
void lumiere::setCentre(vec3 centre) {
	this->centre = centre;
}
void lumiere::setIntensite(double intensite) {
	this->intensite = intensite;
}
vec3 lumiere::getCentre() {
	return this->centre;
}
vec3 lumiere::getCouleur() {
	return this->couleur;
}
double lumiere::getIntensite() {
	return this->intensite;
}

lumiere::~lumiere() {}