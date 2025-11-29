import bcrypt from "bcryptjs";

const plainPassword = "123456";   // Thunder'da kullandığın şifre

const hashPassword = async () => {
  const hash = await bcrypt.hash(plainPassword, 10);
  console.log("Yeni hash:", hash);
};

hashPassword();
