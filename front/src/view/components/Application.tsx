import * as React from "react";
import { useEffect, useMemo } from "react";
import "./Application.scss";
import Login from "@mui/icons-material/Login";
import Logout from "@mui/icons-material/Logout";
import { Todos } from "./test/Todos";
import { useAppDispatch, useAppSelector } from "../../store";
import { toggleTheme } from "../../store/module/theme/theme.action";
import { createDrawerAction, withDrawer } from "./utils/drawer/Drawer.hoc";
import { Box } from "@mui/material";
import { bindActionCreators } from "redux";
import { DarkMode, LightMode } from "@mui/icons-material";
import { login, logout } from "../../store/module/authentication/authentication.async.action";
import { initApp } from "../../store/module/workflow/workflow.async.actions";

function Application() {
	const dispatch = useAppDispatch();

	const { theme, themeIcon, logged } = useAppSelector((s) => ({
		theme: s.theme.current,
		themeIcon: s.theme.current === "dark" ? <LightMode /> : <DarkMode />,
		logged: s.authentication.logged,
	}));

	const storeActions = React.useMemo(() => bindActionCreators({ toggleTheme, logout, login }, dispatch), [dispatch]);

	const actions = useMemo(() => {
		const arr = [
			createDrawerAction(theme === "dark" ? "Light Mode" : "Dark Mode", {
				icon: themeIcon,
				onClick: () => storeActions.toggleTheme(),
			}),
		];
		if (logged) {
			arr.push(
				createDrawerAction("Logout", {
					icon: <Logout fill={"currentColor"} />,
					onClick: storeActions.logout,
				})
			);
		} else {
			arr.push(
				createDrawerAction("Login", {
					icon: <Login fill={"currentColor"} />,
					onClick: storeActions.login,
				})
			);
		}
		return arr;
	}, [theme, logged, storeActions]);

	const drawer = withDrawer({
		component: <Todos />,
		actions,
		title: "Todos",
	});

	useEffect(() => {
		dispatch(initApp());
	}, [dispatch]);

	return (
		<Box className={"Application"} bgcolor={"background.default"}>
			{drawer}
		</Box>
	);
}

export default Application;
