#!/usr/bin/env bash
set -e

APP_NAME="utilitiesmanager"
APP_VERSION="0.9.0-prealpha2"
ARCH="amd64"

# project file
PROJECT_PATH="./UtilitiesManager.csproj"

# Output directories
PUBLISH_DIR="./publish"
DEB_DIR="./Package"
DEBIAN_DIR="$DEB_DIR/DEBIAN"
USR_BIN_DIR="$DEB_DIR/usr/bin"
APP_SHARE_DIR="$DEB_DIR/usr/share/UtilitiesManager"
ICON_DIR="$DEB_DIR/usr/share/icons/hicolor/256x256/apps"

echo "==> Cleaning old builds"
rm -rf "$PUBLISH_DIR"
rm -rf "$DEB_DIR"
rm -f Package.deb
rm -f utilitiesmanager_*.deb

echo "==> Publishing Avalonia app (AOT)"
dotnet publish "$PROJECT_PATH" -c Release -r linux-x64 /p:PublishAot=true -o "$PUBLISH_DIR"

echo "==> Creating Debian package structure"
mkdir -p "$DEBIAN_DIR"
mkdir -p "$USR_BIN_DIR"
mkdir -p "$APP_SHARE_DIR"
mkdir -p "$DEB_DIR/usr/share/applications"
mkdir -p "$ICON_DIR"

echo "==> Copying all published files and native assets (SkiaSharp)"
# Copy everything so native .so files stay next to the binary
cp -r "$PUBLISH_DIR"/* "$APP_SHARE_DIR/"

echo "==> Creating symlinks for commands"
ln -sf "/usr/share/UtilitiesManager/UtilitiesManager" "$USR_BIN_DIR/$APP_NAME"
ln -sf "/usr/share/UtilitiesManager/UtilitiesManager" "$USR_BIN_DIR/UtilMan"

echo "==> Copying icon"
if [ -f "./Assets/UtilManagerV3.png" ]; then
    cp "./Assets/UtilManagerV3.png" "$ICON_DIR/utilitiesmanager.png"
fi

echo "==> Creating .desktop file"
cat <<EOF > "$DEB_DIR/usr/share/applications/$APP_NAME.desktop"
[Desktop Entry]
Name=Utilities Manager
GenericName=System Utilities
Comment=Manage Brightness, Volume, WiFi, Bluetooth and Battery
Exec=UtilMan
Icon=utilitiesmanager
Type=Application
Categories=Utility;System;Settings;
Terminal=false
StartupWMClass=UtilitiesManager
Keywords=System;Utility;Settings;Battery;Brightness;Volume;WiFi;Bluetooth;UtilMan;
EOF

echo "==> Creating control file"
DEPENDS="brightnessctl, pulseaudio-utils, upower, network-manager, power-profiles-daemon, bluez"
cat <<EOF > "$DEBIAN_DIR/control"
Package: $APP_NAME
Version: $APP_VERSION
Section: utils
Priority: optional
Architecture: $ARCH
Maintainer: Alexander
Depends: $DEPENDS
Description: System utility manager with GUI and CLI interfaces
EOF

echo "==> Creating post-installation scripts"
cat <<EOF > "$DEBIAN_DIR/postinst"
#!/bin/sh
set -e
if [ "\$1" = "configure" ]; then
    update-desktop-database -q /usr/share/applications || true
    gtk-update-icon-cache -q -t -f /usr/share/icons/hicolor || true
fi
EOF

cat <<EOF > "$DEBIAN_DIR/postrm"
#!/bin/sh
set -e
if [ "\$1" = "remove" ] || [ "\$1" = "purge" ]; then
    update-desktop-database -q /usr/share/applications || true
    gtk-update-icon-cache -q -t -f /usr/share/icons/hicolor || true
fi
EOF

chmod 555 "$DEBIAN_DIR/postinst"
chmod 555 "$DEBIAN_DIR/postrm"

echo "==> Finalizing permissions"
find "$DEB_DIR" -type d -exec chmod 755 {} +
find "$DEB_DIR" -type f -exec chmod 644 {} +
chmod 755 "$APP_SHARE_DIR/UtilitiesManager"
chmod 755 "$DEBIAN_DIR/postinst" "$DEBIAN_DIR/postrm"

echo "==> Building .deb package"
OUTPUT_FILE="${APP_NAME}_${APP_VERSION}_${ARCH}.deb"
dpkg-deb --root-owner-group --build "$DEB_DIR" "$OUTPUT_FILE"

echo "==> Done!"
echo "Created package: $OUTPUT_FILE"