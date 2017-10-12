from Ray import Ray
from Ray import Sphere
from Ray import Scene
from Tp1 import Vec3
from utils import *
from math import *

def main():
	h = 500
	w = 500
	
	fov = 60*3.1415/180
	cameraPosition = Vec3(0,0,0)
	
	scene = Scene([])
	r1 = Ray(Vec3(0,0,0),Vec3(0,0,1))
	
	s1 = Sphere(Vec3(0,0,-55),20,Vec3(254,49,4), True)
	s2 = Sphere(Vec3(0,-2000-20,0),2000,Vec3(254,231,4), False)
	s3 = Sphere(Vec3(0,2000+100,0),2000,Vec3(6,254,4), False)
	s4 = Sphere(Vec3(-2000-50,0,0),2000,Vec3(4,113,254), False)
	s5 = Sphere(Vec3(2000+50,0,0),2000,Vec3(244,4,254), False)
	s6 = Sphere(Vec3(0,0,-2000-100),20000,Vec3(4,249,254), False)
	
	scene.ajout_sphere(s1)
	scene.ajout_sphere(s2)
	scene.ajout_sphere(s3)
	scene.ajout_sphere(s4)
	scene.ajout_sphere(s5)
	scene.ajout_sphere(s6)
	
	#création du fichier image
	fichier = open("image.ppm", "w")
	fichier.write("P3\n")
	fichier.write(str(w))
	fichier.write(" ")
	fichier.write(str(h))
	fichier.write("\n")
	fichier.write("255\n") 
	
	for i in range(0,h):
		for j in range(0,w):
		
			# calcul de ray.dir
			x = j - w/2
			y = -i + h/2
			z = -w/(2 * (tan(fov /2)))
			direct = Vec3(x,y,z)._normalized_()
			ray=Ray(cameraPosition,direct)

			t = scene.intersection_spheres(ray,fichier)
			
	fichier.close()	
