import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import SaveIcon from '@material-ui/icons/Save';
import ArrowUpwardIcon from '@material-ui/icons/ArrowUpward';
import ArrowDownwardIcon from '@material-ui/icons/ArrowDownward';
import classes from './TorrentCard.module.css';
import Divider from "@material-ui/core/Divider/Divider";

const TorrentCard = (props) => {

    const onDownloadClick = () => window.open(props.magnetUri, '_self');

    return (
        <Card className={props.className}>
            <CardContent>
                <Typography variant="h6">{props.name}</Typography>

                <Divider/>

                <Typography className={classes.uploadedOn} color='textSecondary'>
                    Uploaded On: {props.uploadedOn}
                </Typography>

                <div className={classes.content}>
                    <div className={classes.stat}>
                        <SaveIcon/>
                        <span>{props.size}</span>
                    </div>
                    <div className={classes.stat}>
                        <ArrowUpwardIcon/>
                        <span>{props.seeders}</span>
                    </div>
                    <div className={classes.stat}>
                        <ArrowDownwardIcon/>
                        <span>{props.leechers}</span>
                    </div>
                </div>
            </CardContent>
            <CardActions>
                <Button
                    onClick={onDownloadClick}
                    size="small">Download</Button>
            </CardActions>
        </Card>
    );
};

TorrentCard.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default TorrentCard;