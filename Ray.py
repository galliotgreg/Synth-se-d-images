from Tp1 import Vec3
from math import *
class Ray :#Classe représentant les rayons
	"""
	Attributs : origine et direction du rayons
	Constructeurs
	Accesseurs
	Fonction évaluant la position 3D pour une valeur de t (r = x0 + t*u)
	"""
	
	def __init__(self, orig, direct):
		self.orig = orig
		self.direct = direct
	
	def	_get_orig_(self):
		return self.orig
	def _set_orig_(self,a):
		self.orig = a
	
	def _get_dir_(self):
		return self.direct
	def _set_dir(self, direct):
		self.direct = direct
	
	def _position_3D(self,t):
		r = self.orig._addition_(self.direct._multiplication_float_(t))
		return r
		
	def print_ray(self):
		print("Origine : (",self.orig.x,",",self.orig.y,",",self.orig.z,")"," Direction : (",self.direct.x,",",self.direct.y,",",self.direct.z,")" )

class Sphere :#Classe représentant les sphères
	"""
	Attributs : centre et rayon de la sphères
	Attibuts ajoutés : couleur, mirroir
	Constructeurs
	Accesseurs
	Fonction de calcul d'intersection entre le rayon et la sphere
	"""
	
	def __init__(self, centre, rayon, couleur, bool):
		self.c = centre
		self.r = rayon
		self.couleur = couleur
		self.is_mirror = bool
	
	def _get_r_(self):
		return self.r
	def _set_r_(self, rayon):
		self.r = rayon	
	def _get_c_(self):
		return self.c
	def _set_c_(self, centre):
		self.c = centre
	def get_is_mirror():
		return self.is_mirror()
	def set_is_mirror(self, bool):
		self.is_mirror = bool	
	
	# Calcul la direction du rayon refracté
	def calcul_rayon_r(self, rayon, t):
		# on prend le point d'intersection du rayon sur la sphère auquel on soustrait l'origine de la sphère
		n = rayon._position_3D(t)._soustraction_(self._get_c_())._normalized_()# a normaliser
		
		produit_scal = rayon.direct._produit_scalaire_(n)
		r = rayon.direct._soustraction_(n._multiplication_float_(produit_scal*2))
		return r
	
	def reflect(self,t):
		#inter = Intersection(tb, self.couleur)
		rayon_r = Ray(rayon._position_3D(tb),self.calcul_rayon_r(rayon,tb))	
		inter_temp = self._intersection_rayon_sphere_(rayon_r, self.couleur)
		if inter_temp is not None:
			inter.color = inter_temp.color
			inter.t = inter_temp.t
			return inter
	
	# retourne une intersection (t et la couleur de la sphère) si il y a une intersection entre le rayon et la sphère
	# t est un float
	# couleur est un Vec3
	def _intersection_rayon_sphere_(self,rayon, couleur):
		a = 1
		
		vec = rayon._get_orig_()._soustraction_(self.c)
		dirNorm = rayon._get_dir_()._normalized_()
		produit_scal = dirNorm._produit_scalaire_(vec)
		b = 2*produit_scal
		
		c = rayon.orig._soustraction_(self.c)._norme_au_carre_() - self.r**2
		
		delta = b**2 - 4*a*c
		if delta < 0 :
			return None
		if delta > 0:
			ta = (-b+sqrt(delta))/2*a
			tb = (-b-sqrt(delta))/2*a
			if tb > 0:
				if self.is_mirror:
					#return reflect(self,tb)
					inter = Intersection(tb, self.couleur)
					rayon_r = Ray(rayon._position_3D(tb),self.calcul_rayon_r(rayon,tb))	
					inter_temp = self._intersection_rayon_sphere_(rayon_r, self.couleur)
					if inter_temp is not None:
						inter.color = inter_temp.color
						inter.t = inter_temp.t
						return inter
				else:
					inter = Intersection(tb, self.couleur)
					return inter
			else : 
				if ta > 0 :
					if self.is_mirror:
						inter = Intersection(ta, self.couleur)
						rayon_r = Ray(rayon._position_3D(ta),self.calcul_rayon_r(rayon,tb))						
						inter_temp = self._intersection_rayon_sphere_(rayon_r, self.couleur)
						if inter_temp is not None:
							inter.color = inter_temp.color
							inter.t = inter_temp.t
							return inter
					else:
						inter = Intersection(ta, self.couleur)
						return inter
				else : 
					return None
		elif delta == 0:
			t = -b/2*a
			if t > 0 :
				if self.is_mirror:
					inter = Intersection(t, self.couleur)				
					rayon_r = Ray(rayon._position_3D(t),self.calcul_rayon_r(rayon,tb))
					inter_temp = self._intersection_rayon_sphere_(rayon_r, self.couleur)
					if inter_temp is not None:
						inter.color = inter_temp.color
						inter.t = inter_temp.t
						return inter
				else:
					inter = Intersection(t, self.couleur)
					return inter
			else :
				return None
		
		
class Intersection:
	def __init__(self,t,color):
		self.t = t
		self.color = color
		
class Scene:
	"""
	Attributs : Tableau de sphères
	Constructeur
	Accesseurs
	Fonction d'ajout d'une sphère à la scène
	Fonction de calcul d'intersection de toute les spheres pour trouver la couleur de chaque pixel à afficher
	"""
	def __init__(self, spheres):
		self.spheres = spheres
	
	def ajout_sphere(self, sphere):
		self.spheres.append(sphere)
	
	def intersection_spheres(self, rayon, fichier):
		t = None
		t_temp = None
		color = Vec3(0,0,0)
		color_temp = Vec3(0,0,0)
		write_in_file = False
		for sphere in self.spheres:
			inter = sphere._intersection_rayon_sphere_(rayon, sphere.couleur)
			#t_temp = sphere._intersection_rayon_sphere_(rayon, sphere.color)
			#color_temp = sphere.couleur
			if inter is not None : 
				if inter.t is not None:
				#if t_temp is not None :
					# première initialisation de t
					if t is None :
						t = inter.t
						#t = t_temp
						color = inter.color
						write_in_file = True
					elif inter.t < t : 
						t = inter.t
						color = inter.color
		if write_in_file :
			c1 = color._get_x()		
			c2 = color._get_y()
			c3 = color._get_z()
			fichier.write(str(c1))
			fichier.write(" ")
			fichier.write(str(c2))
			fichier.write(" ")
			fichier.write(str(c3))
			fichier.write(" ")
			fichier.write("\n")

def test_rayon_reflechi():
	r1 = Ray(Vec3(0,0,0),Vec3(1,0,0))
	s1 = Sphere(Vec3(5,0,0),3,Vec3(10,50,90),True)
	s2 = Sphere(Vec3(-5,0,0),3,Vec3(20,120,70), False)
	t = 2
	r2 = Ray(r1._position_3D(t),s1.calcul_rayon_r(r1,t))
	r2.print_ray() # on attend orig  :(2,0,0) dir : (-1,0,0)
	
	h = 500
	w = 500
	
	#création du fichier image
	fichier = open("image.ppm", "w")
	fichier.write("P3\n")
	fichier.write(str(w))
	fichier.write(" ")
	fichier.write(str(h))
	fichier.write("\n")
	fichier.write("255\n")
	
	scene = Scene([])
	scene.ajout_sphere(s1)
	scene.ajout_sphere(s2)
	
	t = scene.intersection_spheres(r1,fichier)
	
	fichier.close()
	print(str(t))

def test_rayon_reflechi2():
	r1 = Ray(Vec3(5,0,0),Vec3(1,0,0))
	s1 = Sphere(Vec3(10,0,0),3,Vec3(10,50,90),True)
	s2 = Sphere(Vec3(0,0,0),3,Vec3(20,120,70), False)
	t = 7
	r2 = Ray(r1._position_3D(t),s1.calcul_rayon_r(r1,t))
	r2.print_ray() # on attend orig  :(7,0,0) dir : (-1,0,0)
	
	h = 500
	w = 500
	
	#création du fichier image
	fichier = open("image.ppm", "w")
	fichier.write("P3\n")
	fichier.write(str(w))
	fichier.write(" ")
	fichier.write(str(h))
	fichier.write("\n")
	fichier.write("255\n")
	
	scene = Scene([])
	scene.ajout_sphere(s1)
	scene.ajout_sphere(s2)
	
	t1 = scene.intersection_spheres(r1,fichier)
	
	fichier.close()
	print(str(t))	
	
			
def testScene():
	scene = Scene([])
	r1 = Ray(Vec3(0,0,0),Vec3(0,0,1))
	s1 = Sphere(Vec3(0,-20,-55),20,Vec3(254,49,4),True)
	s2 = Sphere(Vec3(0,-2000-20,0),2000,Vec3(254,231,4),False)
	s3 = Sphere(Vec3(0,2000+100,0),2000,Vec3(6,254,4),False)
	s4 = Sphere(Vec3(-2000-50,0,0),2000,Vec3(4,113,254),False)
	s5 = Sphere(Vec3(2000+50,0,0),2000,Vec3(244,4,254),False)
	s6 = Sphere(Vec3(0,0,-2000-100),20000,Vec3(4,249,254),False)
	
	scene.ajout_sphere(s1)
	scene.ajout_sphere(s2)
	scene.ajout_sphere(s3)
	scene.ajout_sphere(s4)
	scene.ajout_sphere(s5)
	scene.ajout_sphere(s6)
	
	h = 500
	w = 500
	
	fichier = open("image.ppm", "w")
	fichier.write("P3\n")
	fichier.write(str(w))
	fichier.write(" ")
	fichier.write(str(h))
	fichier.write("\n")
	fichier.write("255\n")
	
	scene.intersection_spheres(r1,fichier)
	
	fichier.close()
	
def test():
	r1=Ray(Vec3(0,0,0),Vec3(1,0,0))
	s1=Sphere(Vec3(5,0,0),3)
	t=s1._intersection_rayon_sphere_(r1)
	print(t) # on attend : 2
	s1=Sphere(Vec3(5,3,0),3)
	t=s1._intersection_rayon_sphere_(r1)
	print(t) # on attend : 5
	r1=Ray(Vec3(0,0,0),Vec3(5,1,0))
	t=s1._intersection_rayon_sphere_(r1)
	print(t) # on attend : 3.22
	r1=Ray(Vec3(0,0,0),Vec3(-5,1,0))
	t=s1._intersection_rayon_sphere_(r1)
	print(t) # on attend : None
	r1=Ray(Vec3(5,3,0),Vec3(1,0,0))
	t=s1._intersection_rayon_sphere_(r1)
	print(t) # on attend : 3
	
