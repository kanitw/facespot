
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../bin/Debug/FaceSpot.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Debug

FSPOT_BLING_DLL_SOURCE=../../../../../usr/lib/f-spot/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll.config
GIO_SHARP_DLL_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll
GIO_SHARP_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll.config
FACESPOT_DLL_MDB_SOURCE=../bin/Debug/FaceSpot.dll.mdb
FACESPOT_DLL_MDB=$(BUILD_DIR)/FaceSpot.dll.mdb
FACESPOT_DLL=

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = ../bin/Release/FaceSpot.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Release

FSPOT_BLING_DLL_SOURCE=../../../../../usr/lib/f-spot/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gtk-sharp-beans.dll.config
GIO_SHARP_DLL_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll
GIO_SHARP_DLL_CONFIG_SOURCE=../../../../../usr/lib/f-spot/gio-sharp.dll.config
FACESPOT_DLL_MDB=
FACESPOT_DLL_SOURCE=../bin/Debug/FaceSpot.dll
FACESPOT_DLL=$(BUILD_DIR)/FaceSpot.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FSPOT_BLING_DLL) \
	$(GTK_SHARP_BEANS_DLL) \
	$(GTK_SHARP_BEANS_DLL_CONFIG) \
	$(GIO_SHARP_DLL) \
	$(GIO_SHARP_DLL_CONFIG) \
	$(FACESPOT_DLL_MDB) \
	$(FACESPOT_DLL)  

LINUX_PKGCONFIG = \
	$(FACESPOT_PC)  


RESGEN=resgen2

FSPOT_BLING_DLL = $(BUILD_DIR)/FSpot.Bling.dll
GTK_SHARP_BEANS_DLL = $(BUILD_DIR)/gtk-sharp-beans.dll
GTK_SHARP_BEANS_DLL_CONFIG = $(BUILD_DIR)/gtk-sharp-beans.dll.config
GIO_SHARP_DLL = $(BUILD_DIR)/gio-sharp.dll
GIO_SHARP_DLL_CONFIG = $(BUILD_DIR)/gio-sharp.dll.config
FACESPOT_PC = $(BUILD_DIR)/facespot.pc

FILES = \
	AssemblyInfo.cs \
	db/Face.cs \
	db/FaceSpotDb.cs \
	db/FaceStore.cs \
	ui/FaceSelectionWidget.cs \
	ui/FaceBrowser.cs \
	ui/FaceWidget.cs 

DATA_FILES = 

RESOURCES = \
	FaceSpot.addin.xml,FaceSpot.FaceSpot.addin.xml \
	ui/FaceBrowser.glade,FaceSpot.ui.FaceBrowser.glade 

EXTRAS = \
	db \
	facespot.pc.in 

REFERENCES =  \
	System \
	-pkg:gtk-sharp-2.0 \
	Mono.Posix \
	-pkg:f-spot \
	-pkg:glade-sharp-2.0 \
	Mono.Data.SqliteClient

DLL_REFERENCES =  \
	../../../../../usr/lib/f-spot/FSpot.Bling.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,FSPOT_BLING_DLL))
$(eval $(call emit-deploy-target,GTK_SHARP_BEANS_DLL))
$(eval $(call emit-deploy-target,GTK_SHARP_BEANS_DLL_CONFIG))
$(eval $(call emit-deploy-target,GIO_SHARP_DLL))
$(eval $(call emit-deploy-target,GIO_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-wrapper,FACESPOT_PC,facespot.pc))
$(eval $(call emit-deploy-target,FACESPOT_DLL))


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
	$(call cp,$(FSPOT_BLING_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GTK_SHARP_BEANS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GTK_SHARP_BEANS_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GIO_SHARP_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GIO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FACESPOT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(FACESPOT_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call cp,$(FACESPOT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FSPOT_BLING_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GTK_SHARP_BEANS_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GTK_SHARP_BEANS_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GIO_SHARP_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GIO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FACESPOT_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FACESPOT_PC),$(DESTDIR)$(libdir)/pkgconfig)
	$(call rm,$(FACESPOT_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
