
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../bin/Debug/FaceSpotSidebar.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../bin/Debug/FaceSpot.dll
BUILD_DIR = ../bin/Debug

FACESPOT_DLL_SOURCE=../bin/Debug/FaceSpot.dll
FSPOT_BLING_DLL_SOURCE=../../../../../usr/lib/f-spot/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll.config
GIO_SHARP_DLL_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll
GIO_SHARP_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll.config
FACESPOTSIDEBAR_DLL_MDB_SOURCE=../bin/Debug/FaceSpotSidebar.dll.mdb
FACESPOTSIDEBAR_DLL_MDB=$(BUILD_DIR)/FaceSpotSidebar.dll.mdb
FACESPOTSIDEBAR_DLL=

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = ../bin/Release/FaceSpotSidebar.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../bin/Release/FaceSpot.dll
BUILD_DIR = ../bin/Release

FACESPOT_DLL_SOURCE=../bin/Debug/FaceSpot.dll
FSPOT_BLING_DLL_SOURCE=../../../../../usr/lib/f-spot/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll.config
GIO_SHARP_DLL_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll
GIO_SHARP_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll.config
FACESPOTSIDEBAR_DLL_MDB=
FACESPOTSIDEBAR_DLL_SOURCE=../bin/Debug/FaceSpotSidebar.dll
FACESPOTSIDEBAR_DLL=$(BUILD_DIR)/FaceSpotSidebar.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FACESPOT_DLL) \
	$(FSPOT_BLING_DLL) \
	$(GTK_SHARP_BEANS_DLL) \
	$(GTK_SHARP_BEANS_DLL_CONFIG) \
	$(GIO_SHARP_DLL) \
	$(GIO_SHARP_DLL_CONFIG) \
	$(FACESPOTSIDEBAR_DLL_MDB) \
	$(FACESPOTSIDEBAR_DLL)  

LINUX_PKGCONFIG = \
	$(FACESPOTSIDEBAR_PC)  


RESGEN=resgen2

FACESPOT_DLL = $(BUILD_DIR)/FaceSpot.dll
FSPOT_BLING_DLL = $(BUILD_DIR)/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL = $(BUILD_DIR)/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG = $(BUILD_DIR)/gtk-sharp-beans.dll.config
GIO_SHARP_DLL = $(BUILD_DIR)/gio-sharp.dll
GIO_SHARP_DLL_CONFIG = $(BUILD_DIR)/gio-sharp.dll.config
FACESPOTSIDEBAR_PC = $(BUILD_DIR)/facespotsidebar.pc

FILES = \
	AssemblyInfo.cs \
	FaceSidebar.cs \
	FaceSidebarWidget.cs 

DATA_FILES = 

RESOURCES = \
	FaceSidebar.addin.xml,FaceSpot.FaceSidebar.addin.xml 

EXTRAS = \
	facespotsidebar.pc.in 

REFERENCES =  \
	System \
	-pkg:gtk-sharp-2.0 \
	-pkg:f-spot \
	Mono.Posix

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,FACESPOT_DLL))
$(eval $(call emit-deploy-target,FSPOT_BLING_DLL))
$(eval $(call emit-deploy-target,GTK_SHARP_BEANS_DLL))
$(eval $(call emit-deploy-target,GTK_SHARP_BEANS_DLL_CONFIG))
$(eval $(call emit-deploy-target,GIO_SHARP_DLL))
$(eval $(call emit-deploy-target,GIO_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-wrapper,FACESPOTSIDEBAR_PC,facespotsidebar.pc))
$(eval $(call emit-deploy-target,FACESPOTSIDEBAR_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FACESPOT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FSPOT_BLING_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GTK_SHARP_BEANS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GTK_SHARP_BEANS_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GIO_SHARP_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GIO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FACESPOTSIDEBAR_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(FACESPOTSIDEBAR_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call cp,$(FACESPOTSIDEBAR_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FACESPOT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FSPOT_BLING_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GTK_SHARP_BEANS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GTK_SHARP_BEANS_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GIO_SHARP_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GIO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FACESPOTSIDEBAR_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FACESPOTSIDEBAR_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call rm,$(FACESPOTSIDEBAR_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
