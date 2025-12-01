-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: localhost    Database: nutriplate
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `dailygoals`
--

DROP TABLE IF EXISTS `dailygoals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dailygoals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `goal_date` date NOT NULL,
  `calorie_goal` int DEFAULT '2000',
  `protein_goal` int DEFAULT '100',
  `carbs_goal` int DEFAULT '250',
  `fat_goal` int DEFAULT '70',
  `water_goal` float DEFAULT '2',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_dailygoals_user_id` (`user_id`),
  KEY `idx_dailygoals_goal_date` (`goal_date`),
  CONSTRAINT `dailygoals_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `chk_calorie_goal` CHECK ((`calorie_goal` > 0)),
  CONSTRAINT `chk_carbs_goal` CHECK ((`carbs_goal` >= 0)),
  CONSTRAINT `chk_fat_goal` CHECK ((`fat_goal` >= 0)),
  CONSTRAINT `chk_protein_goal` CHECK ((`protein_goal` >= 0)),
  CONSTRAINT `chk_water_goal` CHECK ((`water_goal` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dailygoals`
--

LOCK TABLES `dailygoals` WRITE;
/*!40000 ALTER TABLE `dailygoals` DISABLE KEYS */;
/*!40000 ALTER TABLE `dailygoals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dailysummary`
--

DROP TABLE IF EXISTS `dailysummary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dailysummary` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `summary_date` date NOT NULL,
  `total_calories` int DEFAULT '0',
  `total_protein` float DEFAULT '0',
  `total_carbs` float DEFAULT '0',
  `total_fat` float DEFAULT '0',
  `total_water` float DEFAULT '0',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_dailysummary_user_id` (`user_id`),
  KEY `idx_dailysummary_summary_date` (`summary_date`),
  CONSTRAINT `dailysummary_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dailysummary`
--

LOCK TABLES `dailysummary` WRITE;
/*!40000 ALTER TABLE `dailysummary` DISABLE KEYS */;
/*!40000 ALTER TABLE `dailysummary` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foods`
--

DROP TABLE IF EXISTS `foods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foods` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `calories` int NOT NULL,
  `protein` float DEFAULT '0',
  `fat` float DEFAULT '0',
  `carbs` float DEFAULT '0',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_foods_name` (`name`),
  CONSTRAINT `chk_food_calories` CHECK ((`calories` >= 0)),
  CONSTRAINT `chk_food_carbs` CHECK ((`carbs` >= 0)),
  CONSTRAINT `chk_food_fat` CHECK ((`fat` >= 0)),
  CONSTRAINT `chk_food_protein` CHECK ((`protein` >= 0))
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foods`
--

LOCK TABLES `foods` WRITE;
/*!40000 ALTER TABLE `foods` DISABLE KEYS */;
INSERT INTO `foods` VALUES (1,'Yumurta',155,13,11,1.1,'2025-11-30 15:43:52'),(2,'Tavuk Göğsü',165,31,3.6,0,'2025-11-30 15:43:52'),(3,'Pirinç (pişmiş)',130,2.7,0.3,28,'2025-11-30 15:43:52'),(4,'Makarna (pişmiş)',131,5,1.1,25,'2025-11-30 15:43:52'),(5,'Zeytinyağı',884,0,100,0,'2025-11-30 15:43:52');
/*!40000 ALTER TABLE `foods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mealfoods`
--

DROP TABLE IF EXISTS `mealfoods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mealfoods` (
  `id` int NOT NULL AUTO_INCREMENT,
  `meal_id` int NOT NULL,
  `food_id` int NOT NULL,
  `quantity` float DEFAULT '1',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_mealfoods_meal_id` (`meal_id`),
  KEY `idx_mealfoods_food_id` (`food_id`),
  CONSTRAINT `mealfoods_ibfk_1` FOREIGN KEY (`meal_id`) REFERENCES `meals` (`id`) ON DELETE CASCADE,
  CONSTRAINT `mealfoods_ibfk_2` FOREIGN KEY (`food_id`) REFERENCES `foods` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mealfoods`
--

LOCK TABLES `mealfoods` WRITE;
/*!40000 ALTER TABLE `mealfoods` DISABLE KEYS */;
INSERT INTO `mealfoods` VALUES (1,4,1,2,'2025-11-30 15:44:10');
/*!40000 ALTER TABLE `mealfoods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mealimages`
--

DROP TABLE IF EXISTS `mealimages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mealimages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `meal_id` int NOT NULL,
  `image_url` varchar(500) NOT NULL,
  `ml_result` json DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `meal_id` (`meal_id`),
  CONSTRAINT `mealimages_ibfk_1` FOREIGN KEY (`meal_id`) REFERENCES `meals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mealimages`
--

LOCK TABLES `mealimages` WRITE;
/*!40000 ALTER TABLE `mealimages` DISABLE KEYS */;
INSERT INTO `mealimages` VALUES (2,5,'https://example.com/images/plate123.jpg','{\"items\": [\"chicken\", \"rice\", \"salad\"], \"confidence\": [0.94, 0.89, 0.78]}','2025-11-30 17:29:33');
/*!40000 ALTER TABLE `mealimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `meals`
--

DROP TABLE IF EXISTS `meals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `meals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `meal_datetime` datetime NOT NULL,
  `meal_type` enum('breakfast','lunch','dinner','snack') NOT NULL,
  `total_calories` int DEFAULT '0',
  `note` varchar(255) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `idx_meals_user_id` (`user_id`),
  KEY `idx_meals_datetime` (`meal_datetime`),
  CONSTRAINT `meals_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  CONSTRAINT `chk_meal_type` CHECK ((`meal_type` in (_utf8mb4'breakfast',_utf8mb4'lunch',_utf8mb4'dinner',_utf8mb4'snack')))
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `meals`
--

LOCK TABLES `meals` WRITE;
/*!40000 ALTER TABLE `meals` DISABLE KEYS */;
INSERT INTO `meals` VALUES (1,3,'2025-11-29 19:42:00','breakfast',320,'Deneme öğün','2025-11-29 21:17:05'),(3,3,'2025-11-29 19:42:00','breakfast',320,'Deneme öğün','2025-11-30 15:24:45'),(4,3,'2025-11-30 20:00:00','snack',250,'daily summary test','2025-11-30 15:29:38'),(5,3,'2025-11-30 18:00:00','dinner',700,'ML tahminli akşam yemeği','2025-11-30 17:29:33');
/*!40000 ALTER TABLE `meals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recommendations`
--

DROP TABLE IF EXISTS `recommendations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recommendations` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `dietitian_id` int DEFAULT NULL,
  `recommendation_text` text NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `user_id` (`user_id`),
  KEY `dietitian_id` (`dietitian_id`),
  CONSTRAINT `recommendations_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `recommendations_ibfk_2` FOREIGN KEY (`dietitian_id`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recommendations`
--

LOCK TABLES `recommendations` WRITE;
/*!40000 ALTER TABLE `recommendations` DISABLE KEYS */;
INSERT INTO `recommendations` VALUES (1,3,NULL,'Bugünkü protein hedefini artırmalısın!','2025-11-30 15:46:56');
/*!40000 ALTER TABLE `recommendations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (3,'Admin'),(2,'Dietitian'),(1,'User');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `full_name` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `password` varchar(255) DEFAULT NULL,
  `role_id` int NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`),
  KEY `role_id` (`role_id`),
  CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  CONSTRAINT `chk_email` CHECK ((`email` like _utf8mb4'%@%'))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Ceren Test','ceren@example.com','123456',1,'2025-11-29 13:55:19'),(3,'Ceren Yeni','ceren2@example.com','654321',1,'2025-11-29 16:43:34'),(4,'Ceren Serbest','ceren@test.com','123456',1,'2025-11-30 17:41:37');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `vw_highcaloriemeals`
--

DROP TABLE IF EXISTS `vw_highcaloriemeals`;
/*!50001 DROP VIEW IF EXISTS `vw_highcaloriemeals`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_highcaloriemeals` AS SELECT 
 1 AS `meal_id`,
 1 AS `user_id`,
 1 AS `meal_type`,
 1 AS `meal_datetime`,
 1 AS `total_calories`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_mealdetail`
--

DROP TABLE IF EXISTS `vw_mealdetail`;
/*!50001 DROP VIEW IF EXISTS `vw_mealdetail`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_mealdetail` AS SELECT 
 1 AS `meal_id`,
 1 AS `user_id`,
 1 AS `meal_datetime`,
 1 AS `meal_type`,
 1 AS `total_calories`,
 1 AS `food_name`,
 1 AS `quantity`,
 1 AS `calories`,
 1 AS `protein`,
 1 AS `carbs`,
 1 AS `fat`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_sustainablechoicestats`
--

DROP TABLE IF EXISTS `vw_sustainablechoicestats`;
/*!50001 DROP VIEW IF EXISTS `vw_sustainablechoicestats`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_sustainablechoicestats` AS SELECT 
 1 AS `meal_id`,
 1 AS `user_id`,
 1 AS `meal_datetime`,
 1 AS `food_name`,
 1 AS `protein`,
 1 AS `fat`,
 1 AS `quantity`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_userdailysummary`
--

DROP TABLE IF EXISTS `vw_userdailysummary`;
/*!50001 DROP VIEW IF EXISTS `vw_userdailysummary`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_userdailysummary` AS SELECT 
 1 AS `user_id`,
 1 AS `summary_date`,
 1 AS `meal_count`,
 1 AS `total_calories`,
 1 AS `total_protein`,
 1 AS `total_carbs`,
 1 AS `total_fat`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `vw_userprogress`
--

DROP TABLE IF EXISTS `vw_userprogress`;
/*!50001 DROP VIEW IF EXISTS `vw_userprogress`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vw_userprogress` AS SELECT 
 1 AS `user_id`,
 1 AS `full_name`,
 1 AS `summary_date`,
 1 AS `total_calories`,
 1 AS `total_protein`,
 1 AS `total_carbs`,
 1 AS `total_fat`,
 1 AS `total_water`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `vw_highcaloriemeals`
--

/*!50001 DROP VIEW IF EXISTS `vw_highcaloriemeals`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_highcaloriemeals` AS select `m`.`id` AS `meal_id`,`m`.`user_id` AS `user_id`,`m`.`meal_type` AS `meal_type`,`m`.`meal_datetime` AS `meal_datetime`,`m`.`total_calories` AS `total_calories` from `meals` `m` where (`m`.`total_calories` > 600) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_mealdetail`
--

/*!50001 DROP VIEW IF EXISTS `vw_mealdetail`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_mealdetail` AS select `m`.`id` AS `meal_id`,`m`.`user_id` AS `user_id`,`m`.`meal_datetime` AS `meal_datetime`,`m`.`meal_type` AS `meal_type`,`m`.`total_calories` AS `total_calories`,`f`.`name` AS `food_name`,`mf`.`quantity` AS `quantity`,`f`.`calories` AS `calories`,`f`.`protein` AS `protein`,`f`.`carbs` AS `carbs`,`f`.`fat` AS `fat` from ((`meals` `m` left join `mealfoods` `mf` on((`m`.`id` = `mf`.`meal_id`))) left join `foods` `f` on((`f`.`id` = `mf`.`food_id`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_sustainablechoicestats`
--

/*!50001 DROP VIEW IF EXISTS `vw_sustainablechoicestats`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_sustainablechoicestats` AS select `m`.`id` AS `meal_id`,`m`.`user_id` AS `user_id`,`m`.`meal_datetime` AS `meal_datetime`,`f`.`name` AS `food_name`,`f`.`protein` AS `protein`,`f`.`fat` AS `fat`,`mf`.`quantity` AS `quantity` from ((`mealfoods` `mf` join `meals` `m` on((`m`.`id` = `mf`.`meal_id`))) join `foods` `f` on((`f`.`id` = `mf`.`food_id`))) where ((`f`.`fat` < 10) and (`f`.`protein` > 5)) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_userdailysummary`
--

/*!50001 DROP VIEW IF EXISTS `vw_userdailysummary`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_userdailysummary` AS select `m`.`user_id` AS `user_id`,cast(`m`.`meal_datetime` as date) AS `summary_date`,count(distinct `m`.`id`) AS `meal_count`,coalesce(sum((`f`.`calories` * `mf`.`quantity`)),0) AS `total_calories`,coalesce(sum((`f`.`protein` * `mf`.`quantity`)),0) AS `total_protein`,coalesce(sum((`f`.`carbs` * `mf`.`quantity`)),0) AS `total_carbs`,coalesce(sum((`f`.`fat` * `mf`.`quantity`)),0) AS `total_fat` from ((`meals` `m` left join `mealfoods` `mf` on((`m`.`id` = `mf`.`meal_id`))) left join `foods` `f` on((`mf`.`food_id` = `f`.`id`))) group by `m`.`user_id`,cast(`m`.`meal_datetime` as date) order by `summary_date` desc */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vw_userprogress`
--

/*!50001 DROP VIEW IF EXISTS `vw_userprogress`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vw_userprogress` AS select `u`.`id` AS `user_id`,`u`.`full_name` AS `full_name`,`ds`.`summary_date` AS `summary_date`,`ds`.`total_calories` AS `total_calories`,`ds`.`total_protein` AS `total_protein`,`ds`.`total_carbs` AS `total_carbs`,`ds`.`total_fat` AS `total_fat`,`ds`.`total_water` AS `total_water` from (`dailysummary` `ds` join `users` `u` on((`u`.`id` = `ds`.`user_id`))) order by `ds`.`summary_date` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-30 21:06:28
