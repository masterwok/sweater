import React from 'react';
import PropTypes from 'prop-types';
import withStyles from "@material-ui/core/es/styles/withStyles";
import SettingsMenuItem from "./SettingsMenuItem/SettingsMenuItem";
import AccountMenuItem from "./AccountMenuItem/AccountMenuItem";
import EllipsisMenuItem from "./EllipsisMenuItem/EllipsisMenuItem";

const styles = theme => ({
    sectionDesktop: {
        display: 'none',
        [theme.breakpoints.up('md')]: {
            display: 'flex',
        },
    },
    sectionMobile: {
        display: 'flex',
        [theme.breakpoints.up('md')]: {
            display: 'none',
        },
    }
});

const MenuItems = (props) => {

    const {classes} = props;

    return (
        <>
            <div className={classes.sectionDesktop}>
                <SettingsMenuItem/>
                <AccountMenuItem
                    isMenuOpen={props.isMenuOpen}
                    onClick={props.onOpenAccountMenu}/>
            </div>

            <div className={classes.sectionMobile}>
                <EllipsisMenuItem
                    onClick={props.onOpenMobileMenu}/>
            </div>
        </>
    );
};

MenuItems.propTypes = {
    isMenuOpen: PropTypes.bool.isRequired
    , onOpenAccountMenu: PropTypes.func.isRequired
    , onOpenMobileMenu: PropTypes.func.isRequired
};

export default withStyles(styles)(MenuItems);
