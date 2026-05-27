#!/usr/bin/env bash
set -e

APP_NAME="utilitiesmanager"
APP_VERSION="0.8.0-prealpha7"
ARCH="amd64"

# project file
PROJECT_PATH="./UtilitiesManager.csproj"

# Output directories
PUBLISH_DIR="./publish"
DEB_DIR="./Package"
DEBIAN_DIR="$DEB_DIR/DEBIAN"
USR_BIN_DIR="$DEB_DIR/usr/bin"
USR_SHARE_DIR="$DEB_DIR/usr/share/UtilitiesManager"
ICON_DIR="$DEB_DIR/usr/share/icons/hicolor/256x256/apps"

echo "==> Cleaning old builds"
rm -rf "$PUBLISH_DIR"
rm -rf "$DEB_DIR"
rm -f Package.deb
rm -f utilitiesmanager_*.deb

echo "==> Publishing Avalonia app"
dotnet publish "$PROJECT_PATH" -c Release -r linux-x64 --self-contained true -o "$PUBLISH_DIR"

echo "==> Creating Debian package structure"
mkdir -p "$DEBIAN_DIR"
mkdir -p "$USR_BIN_DIR"
mkdir -p "$USR_SHARE_DIR"
mkdir -p "$DEB_DIR/usr/share/applications"
mkdir -p "$ICON_DIR"

echo "==> Copying published files"
cp -r "$PUBLISH_DIR"/* "$USR_SHARE_DIR"

echo "==> Copying icon"
cp "./Assets/UtilManagerV3.png" "$ICON_DIR/utilitiesmanager.png"

echo "==> Creating launcher script"
cat <<EOF > "$USR_BIN_DIR/$APP_NAME"
#!/bin/bash
exec /usr/share/UtilitiesManager/UtilitiesManager "\$@"
EOF
chmod +x "$USR_BIN_DIR/$APP_NAME"

echo "==> Creating symlink for common command name"
# Create symlink from UtilMan to utilitiesmanager for easier access
ln -sf "$APP_NAME" "$USR_BIN_DIR/UtilMan"

echo "==> Creating .desktop file"
cat <<EOF > "$DEB_DIR/usr/share/applications/$APP_NAME.desktop"
[Desktop Entry]
Name=Utilities Manager
GenericName=System Utilities
Comment=Manage Brightness, Volume, WiFi, Bluetooth and Battery
Exec=$APP_NAME
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
# Set standard permissions: 755 for dirs, 644 for files
find "$DEB_DIR" -type d -exec chmod 755 {} +
find "$DEB_DIR" -type f -exec chmod 644 {} +
# Restore execution bits for binaries and scripts
chmod 755 "$USR_BIN_DIR/$APP_NAME"
chmod 755 "$USR_SHARE_DIR/UtilitiesManager"
chmod 755 "$DEBIAN_DIR/postinst" "$DEBIAN_DIR/postrm"

echo "==> Building .deb package"
OUTPUT_FILE="${APP_NAME}_${APP_VERSION}_${ARCH}.deb"
dpkg-deb --root-owner-group --build "$DEB_DIR" "$OUTPUT_FILE"

echo "==> Done!"
echo "Created package: $OUTPUT_FILE"
